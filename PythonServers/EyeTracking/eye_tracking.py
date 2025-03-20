import cv2
import os
import socket
import json
import numpy as np

def suppress_stderr():
    devnull_fd = os.open(os.devnull, os.O_WRONLY)
    os.dup2(devnull_fd, 1)
    os.dup2(devnull_fd, 2)
suppress_stderr()

import mediapipe as mp

def get_eye_directions(avg_gaze_ratio: float, avg_vertical_ratio: float) -> dict:
    left  = bool(avg_gaze_ratio < 0.4)
    right = bool(avg_gaze_ratio > 0.6)
    up    = bool(avg_vertical_ratio < 0.3)
    down  = bool(avg_vertical_ratio > 0.5)
    center = not (left or right or up or down)
    return {
        "left": left,
        "right": right,
        "up": up,
        "down": down,
        "center": center
    }

UDP_IP = "127.0.0.1"
UDP_PORT = 4242
SHOW_WINDOW = False

RIGHT_EYE_OUTER = 33
RIGHT_EYE_INNER = 133
LEFT_EYE_OUTER = 362
LEFT_EYE_INNER = 263
RIGHT_PUPIL = 468
LEFT_PUPIL = 473
RIGHT_EYE_TOP = 159
RIGHT_EYE_BOTTOM = 145
LEFT_EYE_TOP = 386
LEFT_EYE_BOTTOM = 374

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

mp_face_mesh = mp.solutions.face_mesh
face_mesh = mp_face_mesh.FaceMesh(refine_landmarks=True)

cap = cv2.VideoCapture(0)

while cap.isOpened():
    ret, frame = cap.read()
    if not ret:
        break

    frame = cv2.flip(frame, 1)
    rgb_frame = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
    results = face_mesh.process(rgb_frame)

    if results.multi_face_landmarks:
        for face_landmarks in results.multi_face_landmarks:
            h, w, _ = frame.shape

            def get_landmark_position(landmark_idx):
                landmark = face_landmarks.landmark[landmark_idx]
                return int(landmark.x * w), int(landmark.y * h)

            right_eye_outer = get_landmark_position(RIGHT_EYE_OUTER)
            right_eye_inner = get_landmark_position(RIGHT_EYE_INNER)
            left_eye_outer = get_landmark_position(LEFT_EYE_OUTER)
            left_eye_inner = get_landmark_position(LEFT_EYE_INNER)
            right_pupil = get_landmark_position(RIGHT_PUPIL)
            left_pupil = get_landmark_position(LEFT_PUPIL)
            right_eye_top = get_landmark_position(RIGHT_EYE_TOP)
            right_eye_bottom = get_landmark_position(RIGHT_EYE_BOTTOM)
            left_eye_top = get_landmark_position(LEFT_EYE_TOP)
            left_eye_bottom = get_landmark_position(LEFT_EYE_BOTTOM)

            def get_gaze_ratio(outer, inner, pupil):
                eye_width = np.linalg.norm(np.array(outer) - np.array(inner))
                return (pupil[0] - outer[0]) / eye_width

            def get_vertical_gaze_ratio(top, bottom, pupil):
                eye_height = np.linalg.norm(np.array(top) - np.array(bottom))
                return (pupil[1] - top[1]) / eye_height

            right_gaze_ratio = get_gaze_ratio(right_eye_outer, right_eye_inner, right_pupil)
            left_gaze_ratio = get_gaze_ratio(left_eye_outer, left_eye_inner, left_pupil)
            right_vertical_ratio = get_vertical_gaze_ratio(right_eye_top, right_eye_bottom, right_pupil)
            left_vertical_ratio = get_vertical_gaze_ratio(left_eye_top, left_eye_bottom, left_pupil)

            avg_gaze_ratio = (right_gaze_ratio + left_gaze_ratio) / 2
            avg_vertical_ratio = (right_vertical_ratio + left_vertical_ratio) / 2

            # print(f"avg_gaze_ratio = {avg_gaze_ratio:.2f}\tavg_vertical_ratio = {avg_vertical_ratio:.2f}", end='\r')

            eye_directions = get_eye_directions(avg_gaze_ratio, avg_vertical_ratio)

            sock.sendto(json.dumps(eye_directions).encode(), (UDP_IP, UDP_PORT))

            if SHOW_WINDOW:
                cv2.circle(frame, right_pupil, 3, (0, 255, 0), -1)
                cv2.circle(frame, left_pupil, 3, (0, 255, 0), -1)
                display_text = ", ".join([k for k, v in eye_directions.items() if v])
                cv2.putText(frame, display_text, (50, 100), cv2.FONT_HERSHEY_SIMPLEX, 0.8, (0,255,0), 2)
                cv2.putText(frame, f"Horiz: {avg_gaze_ratio:.3f}  Vert: {avg_vertical_ratio:.3f}", (50, 50),
                            cv2.FONT_HERSHEY_SIMPLEX, 0.6, (255, 255, 255), 2)

    if SHOW_WINDOW:
        cv2.imshow("Eye Tracking", frame)
        if cv2.waitKey(1) & 0xFF == ord('q'):
            break

cap.release()
cv2.destroyAllWindows()
