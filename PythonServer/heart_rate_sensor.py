import cv2
import json
from client_base import ClientBase
import logging

logging.basicConfig(
    filename="client_base.log",
    filemode="a",  # Append mode
    format="%(asctime)s - %(levelname)s - %(message)s",
    level=logging.DEBUG  # Use DEBUG level to log everything
)

import random

class HeartRateGenerator:
    def __init__(self, start=90, min_hr=55, max_hr=160, max_step=3):
        self.value = start
        self.min_hr = min_hr
        self.max_hr = max_hr
        self.max_step = max_step

    def get(self):
        step = random.randint(-self.max_step, self.max_step)
        self.value += step
        self.value = max(self.min_hr, min(self.max_hr, self.value))
        return self.value

class HeartRateSensor(ClientBase):
    def __init__(self):
        super().__init__("127.0.0.1", 3800)
        self.handlers = {
            "heart_rate_detection": self.toggle_heart_rate_detection,
        }

        self.process_enable = False
        self.hr_gen = HeartRateGenerator()

    def toggle_heart_rate_detection(self, state):
        self.process_enable = state
        status = "enabled" if state else "disabled"
        logging.info(f"Heart Rate detection {status} {state}")

    def draw_heart_rate_frame(self, frame, heart_rate):
        text = f"{heart_rate}"
        font = cv2.FONT_HERSHEY_SIMPLEX
        scale = 0.8
        thickness = 2
        (w, h), _ = cv2.getTextSize(text, font, scale, thickness)

        x = frame.shape[1] - w - 10
        y = h + 10

        cv2.putText(frame, text, (x, y), font, scale, (0, 0, 255), thickness, cv2.LINE_AA)

    def process(self, frame):
        if not self.process_enable:
            return

        heart_rate = self.hr_gen.get()
        self.draw_heart_rate_frame(frame, heart_rate)