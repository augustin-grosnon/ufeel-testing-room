import torch
import torch.nn as nn
import torch.nn.functional as F
import torchvision.transforms as transforms
from PIL import Image
import cv2
import socket
import json
import os
import mediapipe as mp

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

class EmotionDetector:
    def __init__(self, model_path=None, device=torch.device("cpu")):
        self.device = device
        self.class_labels = ['happiness', 'surprise', 'sadness', 'anger', 'neutral', 'fear']
        self.model = GiMeFive().to(self.device)
        base_dir = os.path.dirname(os.path.abspath(__file__))
        if model_path is None:
            model_path = os.path.join(base_dir, 'models', 'best_GiMeFive.pth')
        self.model.load_state_dict(torch.load(model_path, map_location=self.device))
        self.model.eval()
        self.transform = transforms.Compose([
            transforms.Resize((64, 64)),
            transforms.Grayscale(num_output_channels=3),
            transforms.ToTensor(),
            transforms.Normalize([0.485,0.456,0.406],[0.229,0.224,0.225])
        ])
        self.mp_face_detection = mp.solutions.face_detection
        self.face_detection = self.mp_face_detection.FaceDetection(model_selection=0, min_detection_confidence=0.5)
        self.sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        self.UDP_IP, self.UDP_PORT = "127.0.0.1", 4243
        self.min_score_threshold = 0.1
        self.emotion_diff_threshold = 0.2
        self.default_font_params = dict(fontFace=cv2.FONT_HERSHEY_SIMPLEX, fontScale=1, thickness=2, lineType=cv2.LINE_AA)
        self.selected = []
        self.color_map = {
            'happiness': (0, 255, 0),
            'surprise': (255, 0, 255),
            'sadness':  (255, 0, 0),
            'anger':    (0, 0, 255),
            'fear':     (0, 165, 255),
            'neutral':  (200, 200, 200)
        }

    def detect_emotion(self, img):
        tensor = self.transform(img).unsqueeze(0).to(self.device)
        with torch.no_grad():
            scores = F.softmax(self.model(tensor), dim=1).cpu().numpy().flatten()
        return [round(s, 2) for s in scores]

    def process(self, frame, counter, show_window=True):
        frame_rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        results = self.face_detection.process(frame_rgb)
        if not results.detections:
            return
        height, width, _ = frame.shape
        selected = []
        for detection in results.detections:
            bboxC = detection.location_data.relative_bounding_box
            x = int(bboxC.xmin * width)
            y = int(bboxC.ymin * height)
            w = int(bboxC.width * width)
            h = int(bboxC.height * height)
            x, y = max(0, x), max(0, y)
            w, h = min(width - x, w), min(height - y, h)
            if show_window:
                cv2.rectangle(frame, (x, y), (x+w, y+h), (0, 255, 0), 2)
            crop = frame[y:y+h, x:x+w]
            if crop.size == 0:
                continue
            scores = self.detect_emotion(Image.fromarray(crop))
            emo_dict = {lbl: float(scores[i]) for i, lbl in enumerate(self.class_labels)}
            if counter == 0:
                non_neutral = [(lbl, scores[i]) for i, lbl in enumerate(self.class_labels) if lbl != 'neutral']
                if non_neutral:
                    max_score = max(score for _, score in non_neutral)
                    selected = [lbl for lbl, score in non_neutral if score >= self.min_score_threshold and max_score - score <= self.emotion_diff_threshold]
                else:
                    selected = []
                if selected:
                    self.selected = selected
                self.sock.sendto(json.dumps(emo_dict).encode(), (self.UDP_IP, self.UDP_PORT))
            else:
                selected = self.selected
            if show_window:
                if selected:
                    y_offset = y - 15
                    for i, emotion in enumerate(selected):
                        offset = y_offset - (30 * i)
                        cv2.putText(frame, emotion, (x, offset), **self.default_font_params, color=(0,0,0))
                j = 0
                for i, lbl in enumerate(self.class_labels):
                    if lbl == 'neutral':
                        continue
                    color = self.color_map.get(lbl, (0, 255, 0))
                    pos = (x + w + 10, y - 20 + 40 * (j + 1))
                    cv2.putText(frame, f'{lbl}: {scores[i]:.2f}', pos, **self.default_font_params, color=color)
                    j += 1

    def close(self):
        self.face_detection.close()
