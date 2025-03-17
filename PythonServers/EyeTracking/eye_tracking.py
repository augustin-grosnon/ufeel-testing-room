import cv2, sys, os, socket
import mediapipe as mp
import numpy as np
from enum import Enum

class EyeDirection(Enum):
    CENTER = 0
    LEFT = 1
    RIGHT = 2
    UP = 3
    DOWN = 4

def suppress_stderr():
    devnull_fd = os.open(os.devnull, os.O_WRONLY)
    os.dup2(devnull_fd, 1)
    os.dup2(devnull_fd, 2)

def restore_stderr():
    sys.stdout = sys.__stdout__
    sys.stderr = sys.__stderr__

def get_eye_direction(avg_gaze_ratio_: int, avg_vertical_ratio_: int) -> tuple[EyeDirection, tuple[int, int, int]]:
    if avg_gaze_ratio_ < 0.4:
        return EyeDirection.LEFT, (255, 0, 0)
    if avg_gaze_ratio_ > 0.6:
        return EyeDirection.RIGHT, (0, 0, 255)
    if avg_vertical_ratio_ < 0.4:
        return EyeDirection.UP, (0, 255, 255)
    if avg_vertical_ratio_ > 0.6:
        return EyeDirection.DOWN, (255, 255, 0)
    return EyeDirection.CENTER, (0, 255, 0)

#suppress_stderr()
#restore_stderr()

UDP_IP = "127.0.0.1"
UDP_PORT = 4242
SHOW_WINDOW = True

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

            gaze_direction, color = get_eye_direction(avg_gaze_ratio, avg_vertical_ratio)

            sock.sendto(str(gaze_direction.value).encode(), (UDP_IP, UDP_PORT))

            if SHOW_WINDOW:
                cv2.circle(frame, right_pupil, 3, (0, 255, 0), -1)
                cv2.circle(frame, left_pupil, 3, (0, 255, 0), -1)
                cv2.putText(frame, str(gaze_direction), (50, 100), cv2.FONT_HERSHEY_SIMPLEX, 0.8, color, 2)
                cv2.putText(frame, f"Horiz: {avg_gaze_ratio:.3f}  Vert: {avg_vertical_ratio:.3f}", (50, 50),
                            cv2.FONT_HERSHEY_SIMPLEX, 0.6, (255, 255, 255), 2)

    if SHOW_WINDOW:
        cv2.imshow("Eye Tracking", frame)
        if cv2.waitKey(1) & 0xFF == ord('q'):
            break

cap.release()
cv2.destroyAllWindows()
