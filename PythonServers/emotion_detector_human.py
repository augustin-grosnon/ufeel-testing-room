import cv2
import numpy as np
import tensorflow as tf
from tensorflow.keras.models import load_model
import os


class EmotionDetectorHuman:
    def __init__(self, model_path=None):
        base_dir = os.path.dirname(os.path.abspath(__file__))
        if model_path is None:
            model_path = os.path.join(base_dir, 'models', 'fer2013_mini_XCEPTION.119-0.65.hdf5')

        self.model = load_model('models/fer2013_mini_XCEPTION.119-0.65.hdf5', compile=False)
        self.emotion_labels = ['angry', 'disgust', 'fear', 'happy', 'sad', 'surprise', 'neutral']
        self.face_cascade = cv2.CascadeClassifier(cv2.data.haarcascades + "haarcascade_frontalface_default.xml")

    def preprocess_face(self, face_img, target_size=(48, 48)):
        face_img = cv2.resize(face_img, target_size)
        face_img = face_img.astype('float32') / 255.0
        face_img = np.expand_dims(face_img, axis=0)
        face_img = np.expand_dims(face_img, axis=-1)
        return face_img

    def predict_emotion(self, face_img):
        input_tensor = self.preprocess_face(face_img)
        preds = self.model.predict(input_tensor, verbose=0)[0]  # shape: (7,)
        emotions = [
            {"emotion": self.emotion_labels[i], "score": float(np.round(score, 2))}
            for i, score in enumerate(preds)
            if score > 0.1
        ]
        return sorted(emotions, key=lambda x: x["score"], reverse=True)

    def detect_faces(self, frame):
        gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
        faces = self.face_cascade.detectMultiScale(gray, scaleFactor=1.3, minNeighbors=5)
        return faces, gray

    def process(self, frame, show_window=True):
        faces, gray = self.detect_faces(frame)
        for (x, y, w, h) in faces:
            face = gray[y:y + h, x:x + w]
            if face.size == 0:
                continue
            emotions = self.predict_emotion(face)
            if show_window:
                cv2.rectangle(frame, (x, y), (x + w, y + h), (0, 255, 0), 2)
                for i, emo in enumerate(emotions):
                    text = f"{emo['emotion']}: {emo['score']:.2f}"
                    cv2.putText(frame, text, (x, y - 10 - i * 20), cv2.FONT_HERSHEY_SIMPLEX, 0.6, (255, 255, 255), 1)

        return frame
