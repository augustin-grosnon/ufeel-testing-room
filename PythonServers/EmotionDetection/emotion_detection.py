import torch
import torch.nn as nn
import torch.nn.functional as F
import torchvision.transforms as transforms
from PIL import Image
import numpy as np
import cv2
import socket
import json
import os
import sys

def suppress_stderr():
    devnull_fd = os.open(os.devnull, os.O_WRONLY)
    os.dup2(devnull_fd, 1)
    os.dup2(devnull_fd, 2)

def restore_stderr():
    sys.stdout = sys.__stdout__
    sys.stderr = sys.__stderr__

suppress_stderr()
import mediapipe as mp
restore_stderr()

device = torch.device("cpu")

UDP_IP, UDP_PORT = "127.0.0.1", 4243
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

SHOW_WINDOW = True

color_map = {
    'happiness': (0, 255, 0),
    'surprise': (255, 0, 255),
    'sadness':  (255, 0, 0),
    'anger':    (0, 0, 255),
    'fear':     (0, 165, 255),
    'neutral':  (200, 200, 200)
}

MIN_SCORE_THRESHOLD = 0.1
EMOTION_DIFFERENCE_THRESHOLD = 0.2

mp_face_detection = mp.solutions.face_detection
mp_drawing = mp.solutions.drawing_utils
face_detection = mp_face_detection.FaceDetection(model_selection=0, min_detection_confidence=0.5)

class GiMeFive(nn.Module):
    def __init__(self):
        super().__init__()
        self.conv1, self.bn1 = nn.Conv2d(3, 64, 3, padding=1), nn.BatchNorm2d(64)
        self.conv2, self.bn2 = nn.Conv2d(64, 128, 3, padding=1), nn.BatchNorm2d(128)
        self.conv3, self.bn3 = nn.Conv2d(128, 256, 3, padding=1), nn.BatchNorm2d(256)
        self.conv4, self.bn4 = nn.Conv2d(256, 512, 3, padding=1), nn.BatchNorm2d(512)
        self.conv5, self.bn5 = nn.Conv2d(512, 1024, 3, padding=1), nn.BatchNorm2d(1024)
        self.pool = nn.AdaptiveAvgPool2d((1,1))
        self.fc1, self.fc2 = nn.Linear(1024, 2048), nn.Linear(2048, 1024)
        self.dropout1, self.dropout2 = nn.Dropout(0.2), nn.Dropout(0.5)
        self.fc3 = nn.Linear(1024, 6)
    def forward(self, x):
        for conv, bn in [(self.conv1, self.bn1), (self.conv2, self.bn2),
                         (self.conv3, self.bn3), (self.conv4, self.bn4)]:
            x = self.dropout1(F.max_pool2d(F.relu(bn(conv(x))), 2))
        x = F.max_pool2d(F.relu(self.bn5(self.conv5(x))), 2)
        x = self.pool(x).view(x.size(0), -1)
        x = self.dropout2(F.relu(self.fc1(x)))
        return self.fc3(F.relu(self.fc2(x)))

class_labels = ['happiness', 'surprise', 'sadness', 'anger', 'neutral', 'fear']
model = GiMeFive().to(device)

BASE_DIR = os.path.dirname(os.path.abspath(__file__))
MODEL_PATH = os.path.join(BASE_DIR, 'models', 'best_GiMeFive.pth')

model.load_state_dict(torch.load(MODEL_PATH, map_location=device))
model.eval()

transform = transforms.Compose([
    transforms.Resize((64, 64)),
    transforms.Grayscale(num_output_channels=3),
    transforms.ToTensor(),
    transforms.Normalize([0.485, 0.456, 0.406],[0.229, 0.224, 0.225])
])

cap = cv2.VideoCapture(0)
default_font_params = dict(fontFace=cv2.FONT_HERSHEY_SIMPLEX, fontScale=1, thickness=2, lineType=cv2.LINE_AA)
max_emotion = ''

def detect_emotion(img):
    tensor = transform(img).unsqueeze(0).to(device)
    with torch.no_grad():
        scores = F.softmax(model(tensor), dim=1).cpu().numpy().flatten()
    return [round(s, 2) for s in scores]

def detect_faces(frame, counter):
    global max_emotion
    frame_rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
    results = face_detection.process(frame_rgb)

    if not results.detections:
        return []

    height, width, _ = frame.shape
    faces_detected = []

    for detection in results.detections:
        bboxC = detection.location_data.relative_bounding_box
        x = int(bboxC.xmin * width)
        y = int(bboxC.ymin * height)
        w = int(bboxC.width * width)
        h = int(bboxC.height * height)

        x, y = max(0, x), max(0, y)
        w, h = min(width - x, w), min(height - y, h)

        if SHOW_WINDOW:
            cv2.rectangle(frame, (x, y), (x + w, y + h), (0, 255, 0), 2)

        crop = frame[y:y + h, x:x + w]
        if crop.size == 0:
            continue

        scores = detect_emotion(Image.fromarray(crop))
        emo_dict = {lbl: float(scores[i]) for i, lbl in enumerate(class_labels)}

        if counter == 0:
            non_neutral = [(lbl, scores[i]) for i, lbl in enumerate(class_labels) if lbl != 'neutral']
            if non_neutral:
                max_score = max(score for _, score in non_neutral)
                selected = [
                    lbl for lbl, score in non_neutral
                    if score >= MIN_SCORE_THRESHOLD and max_score - score <= EMOTION_DIFFERENCE_THRESHOLD
                ]
                max_emotion = selected
            else:
                max_emotion = []
            sock.sendto(json.dumps(emo_dict).encode(), (UDP_IP, UDP_PORT))

        if SHOW_WINDOW:
            if max_emotion:
                y_offset = y - 15
                for i, emotion in enumerate(max_emotion):
                    offset = y_offset - (30 * i)
                    cv2.putText(frame, emotion, (x, offset),
                                **default_font_params, color=(0, 0, 0))

            j = 0
            for i, lbl in enumerate(class_labels):
                if lbl == 'neutral':
                    continue
                pos = (x + w + 10, y - 20 + 40 * (j + 1))
                cv2.putText(frame, f'{lbl}: {scores[i]:.2f}', pos,
                            **default_font_params, color=color_map.get(lbl, (0, 255, 0)))
                j += 1

        faces_detected.append((x, y, w, h))

    return faces_detected

counter = 0
freq = 5

while True:
    ret, frame = cap.read()
    if not ret:
        break
    frame = cv2.flip(frame, 1)

    detect_faces(frame, counter)

    if SHOW_WINDOW:
        cv2.imshow("Emotion debug", frame)
        if cv2.waitKey(1) & 0xFF == ord("q"):
            break

    counter = (counter + 1) % freq

cap.release()
if SHOW_WINDOW:
    cv2.destroyAllWindows()

face_detection.close()
