emotions_labels = ['angry', 'disgust', 'fear', 'happy', 'sad', 'surprise', 'neutral']


import cv2
import numpy as np
import tensorflow as tf
from tensorflow.keras.models import load_model

# Charger modèle et étiquettes
model = load_model('models/fer2013_mini_XCEPTION.119-0.65.hdf5', compile=False) #48, 48
emotion_labels = ['angry', 'disgust', 'fear', 'happy', 'sad', 'surprise', 'neutral']

def preprocess_face(face_img, target_size=(48, 48)):
    face_img = cv2.resize(face_img, target_size)
    face_img = face_img.astype('float32') / 255.0
    face_img = np.expand_dims(face_img, axis=0)
    face_img = np.expand_dims(face_img, axis=-1)
    return face_img

def predict_emotion(face_img):
    input_tensor = preprocess_face(face_img)
    preds = model.predict(input_tensor)[0]  # shape: (7,)
    emotions = [
        {"emotion": emotion_labels[i], "score": float(np.round(score, 2))}
        for i, score in enumerate(preds)
        if score > 0.1
    ]
    emotions = sorted(emotions, key=lambda x: x["score"], reverse=True)
    return emotions


def detect_faces(frame, face_cascade):
    gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
    faces = face_cascade.detectMultiScale(gray, 1.3, 5)
    return faces, gray

def run_emotion_detection():
    face_cascade = cv2.CascadeClassifier(cv2.data.haarcascades + 'haarcascade_frontalface_default.xml')
    cap = cv2.VideoCapture(0)

    while True:
        ret, frame = cap.read()
        if not ret:
            break

        frame = cv2.flip(frame, 1)

        faces, gray = detect_faces(frame, face_cascade)

        for (x, y, w, h) in faces:
            face = gray[y:y+h, x:x+w]
            emotions = predict_emotion(face)
            if emotions:
                label = f"{emotions[0]['emotion']} ({emotions[0]['score']})"
                cv2.putText(frame, label, (x, y-10), cv2.FONT_HERSHEY_SIMPLEX, 0.9, (0,255,0), 2)
            cv2.rectangle(frame, (x, y), (x+w, y+h), (255,0,0), 2)

        cv2.imshow('Emotion Detection', frame)

        if cv2.waitKey(1) & 0xFF == ord('q'):
            break

    cap.release()
    cv2.destroyAllWindows()

run_emotion_detection()

