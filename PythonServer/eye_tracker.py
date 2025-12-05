import cv2
import socket
import json
import numpy as np
import mediapipe as mp
import threading
from enum import Enum

import logging
from client_base import ClientBase

logging.basicConfig(
    filename="client_base.log",
    filemode="a",  # Append mode
    format="%(asctime)s - %(levelname)s - %(message)s",
    level=logging.DEBUG  # Use DEBUG level to log everything
)

class EyeTrackingError(Enum):
    NO_EYES_DETECTED = 1

class EyeTracker(ClientBase):
    def __init__(self):
        super().__init__("127.0.0.1", 4000)
        self.handlers = {
            "eye_detection": self.toggle_eye_detection,
        }
        self.mp_face_mesh = mp.solutions.face_mesh
        self.face_mesh = self.mp_face_mesh.FaceMesh(refine_landmarks=True)
        self.RIGHT_EYE_OUTER = 33
        self.RIGHT_EYE_INNER = 133
        self.LEFT_EYE_OUTER = 362
        self.LEFT_EYE_INNER = 263
        self.RIGHT_PUPIL = 468
        self.LEFT_PUPIL = 473
        self.RIGHT_EYE_TOP = 159
        self.RIGHT_EYE_BOTTOM = 145
        self.LEFT_EYE_TOP = 386
        self.LEFT_EYE_BOTTOM = 374
        self.json_ratios = None
        self.SHIFT = 0.025

        self.running = True
        self.process_enable = False

    def toggle_eye_detection(self, state):
        self.process_enable = state
        status = "enabled" if state else "disabled"
        logging.info(f"Eye detection {status}")


    def get_eye_directions(self, avg_gaze_ratio: float, avg_vertical_ratio: float) -> dict:
        left  = bool(avg_gaze_ratio < self.json_ratios["left"][0] + self.SHIFT)
        right = bool(avg_gaze_ratio > self.json_ratios["right"][0] - self.SHIFT)
        up    = bool(avg_vertical_ratio < self.json_ratios["up"][1] + self.SHIFT)
        down  = bool(avg_vertical_ratio > self.json_ratios["down"][1] - self.SHIFT)
        center = not (left or right or up or down)
        return {"left": left, "right": right, "up": up, "down": down, "center": center}

    def get_ratios(self, avg_gaze_ratio: float, avg_vertical_ratio: float) -> dict:
        return {"horizontal": avg_gaze_ratio, "vertical": avg_vertical_ratio}

    def read_ratios_from_file(self, filename):
        with open (filename) as f:
            self.json_ratios = json.load(f)
        # TODO: check if the file opening has failed

    def process(self, frame, calibration, show_window=True):
        if not self.process_enable:
            return
        if self.json_ratios is None:
            self.read_ratios_from_file("./PythonServer/eye_tracker_values.json")

        results = self.face_mesh.process(cv2.cvtColor(frame, cv2.COLOR_BGR2RGB))
        if not results.multi_face_landmarks:
            self.send({"error": EyeTrackingError.NO_EYES_DETECTED.value})
            return
        h, w, _ = frame.shape
        for face_landmarks in results.multi_face_landmarks:
            def get_landmark_position(idx):
                lm = face_landmarks.landmark[idx]
                return int(lm.x * w), int(lm.y * h)
            right_eye_outer = get_landmark_position(self.RIGHT_EYE_OUTER)
            right_eye_inner = get_landmark_position(self.RIGHT_EYE_INNER)
            left_eye_outer = get_landmark_position(self.LEFT_EYE_OUTER)
            left_eye_inner = get_landmark_position(self.LEFT_EYE_INNER)
            right_pupil = get_landmark_position(self.RIGHT_PUPIL)
            left_pupil = get_landmark_position(self.LEFT_PUPIL)
            right_eye_top = get_landmark_position(self.RIGHT_EYE_TOP)
            right_eye_bottom = get_landmark_position(self.RIGHT_EYE_BOTTOM)
            left_eye_top = get_landmark_position(self.LEFT_EYE_TOP)
            left_eye_bottom = get_landmark_position(self.LEFT_EYE_BOTTOM)

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
            # if not calibration:
            eye_directions = self.get_eye_directions(avg_gaze_ratio, avg_vertical_ratio)
            # else:
                # eye_directions = self.get_ratios(avg_gaze_ratio, avg_vertical_ratio)
            self.send(eye_directions)
            if show_window:
                cv2.circle(frame, right_pupil, 3, (0, 255, 0), -1)
                cv2.circle(frame, left_pupil, 3, (0, 255, 0), -1)
                display_text = ", ".join([k for k, v in eye_directions.items() if v])
                cv2.putText(frame, display_text, (50, 100), cv2.FONT_HERSHEY_SIMPLEX, 0.8, (0,255,0), 2)
                cv2.putText(frame, f"Horiz: {avg_gaze_ratio:.3f}  Vert: {avg_vertical_ratio:.3f}", (50, 50),
                            cv2.FONT_HERSHEY_SIMPLEX, 0.6, (255, 255, 255), 2)

    def close(self):
        self.face_mesh.close()
