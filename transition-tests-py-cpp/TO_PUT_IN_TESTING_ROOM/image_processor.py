import numpy as np
import torch
import torch.nn as nn
import torch.nn.functional as F
import torchvision.transforms as transforms
from PIL import Image
import cv2
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
            transforms.Resize((64,64)),
            transforms.Grayscale(num_output_channels=3),
            transforms.ToTensor(),
            transforms.Normalize([0.485,0.456,0.406],[0.229,0.224,0.225])
        ])

        self.mp_face_detection = mp.solutions.face_detection
        self.face_detection = self.mp_face_detection.FaceDetection(
            model_selection=0, min_detection_confidence=0.5
        )

    def log_preprocess_step(self, img):
        img.save("step0_original.png")

        resized = img.resize((64,64))
        resized.save("step1_resized.png")

        gray = resized.convert("L")
        gray.save("step2_gray.png")

        gray3 = Image.merge("RGB",[gray,gray,gray])
        gray3.save("step3_gray3.png")

        tensor = transforms.ToTensor()(gray3)
        np.save("step4_tensor.npy", tensor.numpy())

        norm = transforms.Normalize([0.485,0.456,0.406],[0.229,0.224,0.225])
        tensor_norm = norm(tensor)
        np.save("step5_normalized.npy", tensor_norm.numpy())

    def log_preprocess(self, img, label=""):
        tensor = self.transform(img).unsqueeze(0)
        np_tensor = tensor.squeeze(0).numpy()

        c_mean = np.mean(np_tensor, axis=(1,2))
        c_std  = np.std(np_tensor, axis=(1,2))
        print(f"[Python {label}] Tensor shape: {tensor.shape}, mean per channel: {c_mean}, std per channel: {c_std}")

        flattened = np_tensor.flatten()
        print(f"[Python {label}] First 10 flattened values: {flattened[:10]}")
        return tensor

    def detect_emotion(self, img, log=True):
        if log:
            _ = self.log_preprocess_step(img)

        tensor = self.transform(img).unsqueeze(0).to(self.device)
        if log:
            self.log_preprocess(img, label="before model")
        with torch.no_grad():
            scores = F.softmax(self.model(tensor), dim=1).cpu().numpy().flatten()
        if log:
            print(f"[Python] Raw scores: {scores}")
        return [round(s, 2) for s in scores]

    def process_image(self, image_path):
        frame = cv2.imread(image_path)
        if frame is None:
            print(f"Error: could not load '{image_path}'")
            return []

        frame_rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        results = self.face_detection.process(frame_rgb)
        emotions_list = []

        if not results.detections:
            return []

        h, w, _ = frame.shape
        for detection in results.detections:
            bboxC = detection.location_data.relative_bounding_box
            x = max(0, int(bboxC.xmin * w))
            y = max(0, int(bboxC.ymin * h))
            w_box = min(w - x, int(bboxC.width * w))
            h_box = min(h - y, int(bboxC.height * h))
            crop = frame[y:y+h_box, x:x+w_box]
            if crop.size == 0:
                continue
            scores = self.detect_emotion(Image.fromarray(crop))
            emotions_list.append({lbl: float(scores[i]) for i,lbl in enumerate(self.class_labels)})

        return emotions_list

    def close(self):
        self.face_detection.close()


if __name__ == "__main__":
    image_paths = ["face_happy_1.png", "face_anger_1.png"]
    # image_paths = ["face_happy_1.png"]
    detector = EmotionDetector()

    for img_path in image_paths:
        results = detector.process_image(img_path)
        print(f"Results for '{img_path}':")
        if results:
            for i, face_scores in enumerate(results):
                print(f"  Face {i+1}: {face_scores}")
        else:
            print("  No faces detected.")
    detector.close()
