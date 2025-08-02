import cv2
import numpy as np
from tensorflow.keras.models import load_model
import os
import socket
import json

class EmotionDetectorHuman:
    def __init__(self, model_path=None, udp_ip="127.0.0.1", udp_port=4243):
        base_dir = os.path.dirname(os.path.abspath(__file__))
        if model_path is None:
            # model_path = os.path.join(base_dir, 'models', 'fer2013_mini_XCEPTION.119-0.65.hdf5')
            # model_path = os.path.join(base_dir, 'models', 'fer2013_big_XCEPTION.54-0.66.hdf5')
            # model_path = os.path.join(base_dir, 'models', 'simple_CNN_converted_tf.h5') # python 3.12 version
            model_path = os.path.join(base_dir, 'models', 'simple_CNN.985-0.66.hdf5') # python 3.10 version

        self.model = load_model(model_path, compile=False)
        self.emotion_labels = ['anger', 'disgust', 'fear', 'happiness', 'sadness', 'surprise', 'neutral']
        self.face_cascade = cv2.CascadeClassifier(cv2.data.haarcascades + "haarcascade_frontalface_default.xml")

        self.sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        self.UDP_IP = udp_ip
        self.UDP_PORT = udp_port
        self.default_font_params = dict(fontFace=cv2.FONT_HERSHEY_SIMPLEX, fontScale=0.6, thickness=1, lineType=cv2.LINE_AA)
        self.color = (255, 255, 255)

    def preprocess_face(self, face_img, target_size=(48, 48)):
    # def preprocess_face(self, face_img, target_size=(64, 64)):
        face_img = cv2.resize(face_img, target_size)
        face_img = face_img.astype('float32') / 255.0
        face_img = np.expand_dims(face_img, axis=0)
        face_img = np.expand_dims(face_img, axis=-1)
        return face_img

    def predict_emotion(self, face_img):
        input_tensor = self.preprocess_face(face_img)
        preds = self.model.predict(input_tensor, verbose=0)[0]
        return preds

    def detect_faces(self, frame):
        gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
        faces = self.face_cascade.detectMultiScale(gray, scaleFactor=1.3, minNeighbors=5)
        return faces, gray

    def process(self, frame, counter, show_window=True):
        faces, gray = self.detect_faces(frame)
        if len(faces) == 0:
            return frame

        for (x, y, w, h) in faces:
            face = gray[y:y + h, x:x + w]
            if face.size == 0:
                continue

            preds = self.predict_emotion(face)
            emo_dict = {
                self.emotion_labels[i]: float(np.round(score, 2))
                for i, score in enumerate(preds)
            }

            if counter == 0:
                self.sock.sendto(json.dumps(emo_dict).encode(), (self.UDP_IP, self.UDP_PORT))

            if show_window:
                sorted_emotions = sorted(emo_dict.items(), key=lambda x: x[1])

                cv2.rectangle(frame, (x, y), (x + w, y + h), (0, 255, 0), 2)

                for i, (emotion, score) in enumerate(sorted_emotions):
                    text = f"{emotion}: {score:.2f}"
                    cv2.putText(frame, text, (x, y - 10 - i * 20), **self.default_font_params, color=self.color)

        return frame

    def close(self):
        self.sock.close()
