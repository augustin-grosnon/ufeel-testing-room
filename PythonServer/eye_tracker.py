# import cv2
# import json
# import numpy as np
# import mediapipe as mp
# from enum import Enum

# import logging
# from client_base import ClientBase

# logging.basicConfig(
#     filename="client_base.log",
#     filemode="a",
#     format="%(asctime)s - %(levelname)s - %(message)s",
#     level=logging.DEBUG
# )

# class EyeTrackingError(Enum):
#     NO_EYES_DETECTED = 1

# class EyeTracker(ClientBase):
#     def __init__(self):
#         super().__init__("127.0.0.1", 4000)
#         self.handlers = {
#             "eye_detection": self.toggle_eye_detection,
#         }
#         self.mp_face_mesh = mp.solutions.face_mesh
#         self.face_mesh = self.mp_face_mesh.FaceMesh(refine_landmarks=True)
#         self.RIGHT_EYE_OUTER = 33
#         self.RIGHT_EYE_INNER = 133
#         self.LEFT_EYE_OUTER = 362
#         self.LEFT_EYE_INNER = 263
#         self.RIGHT_PUPIL = 468
#         self.LEFT_PUPIL = 473
#         self.RIGHT_EYE_TOP = 159
#         self.RIGHT_EYE_BOTTOM = 145
#         self.LEFT_EYE_TOP = 386
#         self.LEFT_EYE_BOTTOM = 374
#         self.json_ratios = None
#         self.SHIFT = 0.025

#         self.running = True
#         self.process_enable = False

#     def toggle_eye_detection(self, state):
#         self.process_enable = state
#         status = "enabled" if state else "disabled"
#         logging.info(f"Eye detection {status}")


#     def get_eye_directions(self, avg_gaze_ratio: float, avg_vertical_ratio: float) -> dict:
#         left  = bool(avg_gaze_ratio < self.json_ratios["left"][0] + self.SHIFT)
#         right = bool(avg_gaze_ratio > self.json_ratios["right"][0] - self.SHIFT)
#         up    = bool(avg_vertical_ratio < self.json_ratios["up"][1] + self.SHIFT)
#         down  = bool(avg_vertical_ratio > self.json_ratios["down"][1] - self.SHIFT)
#         center = not (left or right or up or down)
#         return {"left": left, "right": right, "up": up, "down": down, "center": center}

#     def get_ratios(self, avg_gaze_ratio: float, avg_vertical_ratio: float) -> dict:
#         return {"horizontal": avg_gaze_ratio, "vertical": avg_vertical_ratio}

#     def read_ratios_from_file(self, filename):
#         with open (filename) as f:
#             self.json_ratios = json.load(f)
#         # TODO: check if the file opening has failed

#     def process(self, frame, calibration, show_window=True):
#         if not self.process_enable:
#             return
#         if self.json_ratios is None:
#             self.read_ratios_from_file("./PythonServer/eye_tracker_values.json")

#         results = self.face_mesh.process(cv2.cvtColor(frame, cv2.COLOR_BGR2RGB))
#         if not results.multi_face_landmarks:
#             self.send({"error": EyeTrackingError.NO_EYES_DETECTED.value})
#             return
#         h, w, _ = frame.shape
#         for face_landmarks in results.multi_face_landmarks:
#             def get_landmark_position(idx):
#                 lm = face_landmarks.landmark[idx]
#                 return int(lm.x * w), int(lm.y * h)
#             right_eye_outer = get_landmark_position(self.RIGHT_EYE_OUTER)
#             right_eye_inner = get_landmark_position(self.RIGHT_EYE_INNER)
#             left_eye_outer = get_landmark_position(self.LEFT_EYE_OUTER)
#             left_eye_inner = get_landmark_position(self.LEFT_EYE_INNER)
#             right_pupil = get_landmark_position(self.RIGHT_PUPIL)
#             left_pupil = get_landmark_position(self.LEFT_PUPIL)
#             right_eye_top = get_landmark_position(self.RIGHT_EYE_TOP)
#             right_eye_bottom = get_landmark_position(self.RIGHT_EYE_BOTTOM)
#             left_eye_top = get_landmark_position(self.LEFT_EYE_TOP)
#             left_eye_bottom = get_landmark_position(self.LEFT_EYE_BOTTOM)

#             def get_gaze_ratio(outer, inner, pupil):
#                 eye_width = np.linalg.norm(np.array(outer) - np.array(inner))
#                 return (pupil[0] - outer[0]) / eye_width

#             def get_vertical_gaze_ratio(top, bottom, pupil):
#                 eye_height = np.linalg.norm(np.array(top) - np.array(bottom))
#                 return (pupil[1] - top[1]) / eye_height

#             right_gaze_ratio = get_gaze_ratio(right_eye_outer, right_eye_inner, right_pupil)
#             left_gaze_ratio = get_gaze_ratio(left_eye_outer, left_eye_inner, left_pupil)
#             right_vertical_ratio = get_vertical_gaze_ratio(right_eye_top, right_eye_bottom, right_pupil)
#             left_vertical_ratio = get_vertical_gaze_ratio(left_eye_top, left_eye_bottom, left_pupil)
#             avg_gaze_ratio = (right_gaze_ratio + left_gaze_ratio) / 2
#             avg_vertical_ratio = (right_vertical_ratio + left_vertical_ratio) / 2
#             # if not calibration:
#             eye_directions = self.get_eye_directions(avg_gaze_ratio, avg_vertical_ratio)
#             # else:
#                 # eye_directions = self.get_ratios(avg_gaze_ratio, avg_vertical_ratio)
#             self.send(eye_directions)
#             if show_window:
#                 cv2.circle(frame, right_pupil, 3, (0, 255, 0), -1)
#                 cv2.circle(frame, left_pupil, 3, (0, 255, 0), -1)
#                 display_text = ", ".join([k for k, v in eye_directions.items() if v])
#                 cv2.putText(frame, display_text, (10, 70), cv2.FONT_HERSHEY_SIMPLEX, 0.8, (0,255,0), 2)
#                 cv2.putText(frame, f"Horiz: {avg_gaze_ratio:.3f}  Vert: {avg_vertical_ratio:.3f}", (10, 30),
#                             cv2.FONT_HERSHEY_SIMPLEX, 0.6, (255, 255, 255), 2)

#     def close(self):
#         if self.face_mesh is not None:
#             self.face_mesh.close()
#             self.face_mesh = None

from collections import deque

import cv2
from enum import Enum

import logging

from eye_tracking.debug_drawer import DebugDrawer
from eye_tracking.eye_analyzer import EyeAnalyzer
from eye_tracking.face_mesh_detector import FaceMeshDetector
from client_base import ClientBase

logging.basicConfig(
    filename="client_base.log",
    filemode="a",
    format="%(asctime)s - %(levelname)s - %(message)s",
    level=logging.DEBUG
)

class EyeTrackingError(Enum):
    NO_EYES_DETECTED = 1

class EyeTracker(ClientBase):
    def __init__(self):
        super().__init__("127.0.0.1", 4000)
        self.handlers = {
            "eye_detection": self.toggle_eye_detection,
        }

        self.running = True
        self.process_enable = False

        self.detector = FaceMeshDetector()
        self.analyzer = EyeAnalyzer()
        self.drawer = DebugDrawer()

        self.blink_state = {
            "counter": 0,
            "is_blinking": False,
            "threshold_frames": 3,
        }

        self.gaze_check = {
            "right": lambda x: x > 0.55,
            "left": lambda x: x < 0.45,
            "center": lambda x: 0.45 <= x <= 0.55
        }

        self.gaze_buffer = deque(maxlen=7)

    def toggle_eye_detection(self, state):
        self.process_enable = state
        status = "enabled" if state else "disabled"
        logging.info(f"Eye detection {status}")

    def process(self, frame, calibration, show_window=True):
        if not self.process_enable:
            return

        landmarks = self._get_landmarks(frame)
        if not landmarks:
            self.send({"error": EyeTrackingError.NO_EYES_DETECTED.value})
            return

        raw_gaze = self.analyzer.detect_gaze(landmarks)
        gaze = self._update_gaze_state(raw_gaze)

        raw_blink = self.analyzer.detect_blink(landmarks)
        blink = self._update_blink_state(raw_blink)

        self.send({
            "right": self.gaze_check["right"](gaze),
            "left": self.gaze_check["left"](gaze),
            "center": self.gaze_check["center"](gaze),
            "blink": blink,
        })

        if show_window:
            self._draw(frame, gaze, blink, landmarks)

    def _update_gaze_state(self, gaze):
        self.gaze_buffer.append(gaze)
        return sum(self.gaze_buffer) / len(self.gaze_buffer)

    def _get_landmarks(self, frame):
        results = self.detector.process(frame)
        return self.detector.get_landmarks(results, frame.shape)

    def _update_blink_state(self, blink_raw: bool) -> bool:
        if blink_raw:
            self.blink_state["counter"] += 1
        else:
            self.blink_state["counter"] = 0
            self.blink_state["is_blinking"] = False

        if self.blink_state["counter"] >= self.blink_state["threshold_frames"]:
            self.blink_state["is_blinking"] = True

        return self.blink_state["is_blinking"]

    def _draw(self, frame, gaze, blink, landmarks):
        if self.gaze_check["right"](gaze):
            gaze_label = "RIGHT"
        elif self.gaze_check["left"](gaze):
            gaze_label = "LEFT"
        else:
            gaze_label = "CENTER"

        cv2.putText(frame, f"Gaze: {gaze_label}", (30, 50),
                    cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 255, 0), 2)

        cv2.putText(frame, f"Blink: {blink}", (30, 100),
                    cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 0, 255), 2)

        if self.drawer is None:
            return

        LEFT_EYE = [33, 133, 160, 144]
        RIGHT_EYE = [362, 263, 387, 373]

        self.drawer.draw_landmarks(frame, landmarks, LEFT_EYE, (0, 255, 0))
        self.drawer.draw_landmarks(frame, landmarks, RIGHT_EYE, (0, 255, 0))

        left_eye = landmarks[33]
        right_eye = landmarks[263]
        nose = landmarks[1]

        eye_center = (
            (left_eye[0] + right_eye[0]) // 2,
            (left_eye[1] + right_eye[1]) // 2,
        )

        self.drawer.draw_point(frame, left_eye, (255, 0, 0), "L")
        self.drawer.draw_point(frame, right_eye, (255, 0, 0), "R")
        self.drawer.draw_point(frame, nose, (0, 0, 255), "N")
        self.drawer.draw_point(frame, eye_center, (255, 255, 0), "C")

        LEFT_IRIS = [468, 469, 470, 471]
        RIGHT_IRIS = [473, 474, 475, 476]

        for i in LEFT_IRIS + RIGHT_IRIS:
            x, y = landmarks[i]
            cv2.circle(frame, (x, y), 2, (0, 255, 255), -1)
