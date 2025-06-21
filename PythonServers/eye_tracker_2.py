import cv2
import mediapipe as mp
import numpy as np
from sklearn.pipeline import make_pipeline
from sklearn.preprocessing import PolynomialFeatures
from sklearn.linear_model import Ridge

mp_face_mesh = mp.solutions.face_mesh
face_mesh = mp_face_mesh.FaceMesh(refine_landmarks=True)

LEFT_EYE = [33, 133]
RIGHT_EYE = [362, 263]
NOSE_TIP = 1

def get_eye_center(landmarks, eye_indices, frame_shape):
    h, w = frame_shape
    eye_coords = [landmarks[i] for i in eye_indices]
    eye_coords_px = [(int(p.x * w), int(p.y * h)) for p in eye_coords]
    center = np.mean(eye_coords_px, axis=0)
    return center

def get_nose_position(landmarks, frame_shape):
    h, w = frame_shape
    nose = landmarks[NOSE_TIP]
    return np.array([nose.x * w, nose.y * h])

calib_targets = {
    "top_left": (0.1, 0.1),
    "top_center": (0.5, 0.1),
    "top_right": (0.9, 0.1),
    "center_left": (0.1, 0.5),
    "center": (0.5, 0.5),
    "center_right": (0.9, 0.5),
    "bottom_left": (0.1, 0.9),
    "bottom_center": (0.5, 0.9),
    "bottom_right": (0.9, 0.9),
}

screen_w, screen_h = 1920, 1080
eye_data = []
screen_data = []

cap = cv2.VideoCapture(0)
print("Calibrating...")

for name, (rx, ry) in calib_targets.items():
    while True:
        ret, frame = cap.read()
        if not ret:
            break
        frame = cv2.flip(frame, 1)
        rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        results = face_mesh.process(rgb)

        disp = np.zeros((screen_h, screen_w, 3), dtype=np.uint8)
        target_x, target_y = int(rx * screen_w), int(ry * screen_h)
        cv2.circle(disp, (target_x, target_y), 15, (0, 0, 255), -1)
        cv2.putText(disp, f"Look at the circle ({name}) and press space", (10, 30),
                    cv2.FONT_HERSHEY_SIMPLEX, 0.6, (255, 255, 255), 1)
        cv2.imshow("Calibration", disp)

        if results.multi_face_landmarks:
            landmarks = results.multi_face_landmarks[0].landmark
            left_eye = get_eye_center(landmarks, LEFT_EYE, frame.shape[:2])
            right_eye = get_eye_center(landmarks, RIGHT_EYE, frame.shape[:2])
            eye_center = (np.array(left_eye) + np.array(right_eye)) / 2
            nose_pos = get_nose_position(landmarks, frame.shape[:2])
            eye_relative = eye_center - nose_pos

        key = cv2.waitKey(1)
        if key == 32:
            print(f"Saved {name}")
            eye_data.append(eye_relative)
            screen_data.append((target_x, target_y))
            break
        elif key == 27:
            cap.release()
            cv2.destroyAllWindows()
            exit()

cv2.destroyWindow("Calibration")

eye_data = np.array(eye_data)
screen_data = np.array(screen_data)
model_x = make_pipeline(PolynomialFeatures(2), Ridge()).fit(eye_data, screen_data[:, 0])
model_y = make_pipeline(PolynomialFeatures(2), Ridge()).fit(eye_data, screen_data[:, 1])

while True:
    ret, frame = cap.read()
    if not ret:
        break
    frame = cv2.flip(frame, 1)
    rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
    results = face_mesh.process(rgb)

    gaze_display = np.zeros((screen_h, screen_w, 3), dtype=np.uint8)

    if results.multi_face_landmarks:
        landmarks = results.multi_face_landmarks[0].landmark
        left_eye = get_eye_center(landmarks, LEFT_EYE, frame.shape[:2])
        right_eye = get_eye_center(landmarks, RIGHT_EYE, frame.shape[:2])
        eye_center = (np.array(left_eye) + np.array(right_eye)) / 2
        nose_pos = get_nose_position(landmarks, frame.shape[:2])
        eye_relative = (eye_center - nose_pos).reshape(1, -1)

        pred_x = int(model_x.predict(eye_relative)[0])
        pred_y = int(model_y.predict(eye_relative)[0])
        cv2.circle(gaze_display, (pred_x, pred_y), 15, (0, 255, 0), -1)

    cv2.imshow("Eye tracking", gaze_display)
    cv2.imshow("Webcam", frame)

    if cv2.waitKey(1) == ord('q'):
        break

cap.release()
cv2.destroyAllWindows()
