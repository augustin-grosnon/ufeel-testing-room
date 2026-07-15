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
    filemode="w",
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
        # logging.info(f"Eye detection {status}")

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
