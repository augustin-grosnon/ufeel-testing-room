import torch, torch.nn as nn, torch.nn.functional as F
import torchvision.transforms as transforms
from PIL import Image
import numpy as np, cv2

device = torch.device("mps" if torch.backends.mps.is_available() else "cpu")

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

class_labels = ['happiness','surprise','sadness','anger','disgust','fear']
model = GiMeFive().to(device)
model.load_state_dict(torch.load('models/best_GiMeFive.pth', map_location=device))
model.eval()

transform = transforms.Compose([
    transforms.Resize((64, 64)),
    transforms.Grayscale(num_output_channels=3),
    transforms.ToTensor(),
    transforms.Normalize([0.485,0.456,0.406], [0.229,0.224,0.225])
])

face_classifier = cv2.CascadeClassifier(cv2.data.haarcascades + "haarcascade_frontalface_default.xml")
cap = cv2.VideoCapture(0)
font_params = dict(fontFace=cv2.FONT_HERSHEY_SIMPLEX, fontScale=1, color=(0,255,0), thickness=2, lineType=cv2.LINE_AA)
max_emotion = ''

def detect_emotion(img):
    tensor = transform(img).unsqueeze(0).to(device)
    with torch.no_grad():
        scores = F.softmax(model(tensor), dim=1).cpu().numpy().flatten()
    return [round(s, 2) for s in scores]

def detect_faces(frame, counter):
    global max_emotion
    gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
    faces = face_classifier.detectMultiScale(gray, 1.1, 5, minSize=(40,40))
    for (x, y, w, h) in faces:
        cv2.rectangle(frame, (x, y), (x+w, y+h), (0,255,0), 2)
        crop = frame[y:y+h, x:x+w]
        scores = detect_emotion(Image.fromarray(crop))
        if counter == 0: max_emotion = class_labels[np.argmax(scores)]
        cv2.putText(frame, max_emotion, (x, y-15), **font_params)
        for i, lbl in enumerate(class_labels):
            pos = (x+w+10, y-20+40*(i+1))
            cv2.putText(frame, f'{lbl}: {scores[i]:.2f}', pos, **font_params)
    return faces

counter, freq = 0, 5
while True:
    ret, frame = cap.read()
    if not ret: break
    detect_faces(frame, counter)
    cv2.imshow("GiMeFive", frame)
    if cv2.waitKey(1) & 0xFF == ord("q"): break
    counter = (counter + 1) % freq

cap.release()
cv2.destroyAllWindows()
