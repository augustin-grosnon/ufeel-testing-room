import threading
import time

import cv2
from client_base import ClientBase
import logging
import requests
import random

logging.basicConfig(
    filename="client_base.log",
    filemode="a",
    format="%(asctime)s - %(levelname)s - %(message)s",
    level=logging.DEBUG
)


class HeartRateGenerator:
    def __init__(self, start=90, min_hr=55, max_hr=160, max_step=3):
        self.value = start
        self.min_hr = min_hr
        self.max_hr = max_hr
        self.max_step = max_step
        self.source = 0 # source: 0 = external sensor, 1 = simulated

        self.sensor_value = -1
        self.running = True
        self.should_fetch = False

        self.thread = threading.Thread(target=self._sensor_loop, daemon=True)
        self.thread.start()

        self.session = requests.Session()

    def get_simulated(self):
        step = random.randint(-self.max_step, self.max_step)
        self.value += step
        self.value = max(self.min_hr, min(self.max_hr, self.value))
        return self.value

    def _sensor_loop(self):
        while self.running:
            if self.source == 0:
                if not self.should_fetch:
                    continue
                self.should_fetch = False
                try:
                    self.sensor_value = self.session.get(
                        "http://heartbeatufeel.local",
                    ).json()["bpm"]
                except Exception:
                    pass

            time.sleep(0.1)

    def get_sensor(self):
        self.should_fetch = True
        return self.sensor_value

class HeartRateSensor(ClientBase):
    def __init__(self):
        super().__init__("127.0.0.1", 3800)
        self.handlers = {
            "heart_rate_detection": self.toggle_heart_rate_detection,
        }

        self.process_enable = False
        self.hr_gen = HeartRateGenerator()

        self.current_heart_rate = -1

    def toggle_heart_rate_detection(self, state):
        self.process_enable = state
        status = "enabled" if state else "disabled"
        logging.info(f"Heart Rate detection {status} {state}")

    def change_data_source(self, source):
        self.hr_gen.source = source

    def draw_heart_rate_frame(self, frame, heart_rate):
        text = f"{heart_rate}"
        font = cv2.FONT_HERSHEY_SIMPLEX
        scale = 0.8
        thickness = 2
        (w, h), _ = cv2.getTextSize(text, font, scale, thickness)

        x = frame.shape[1] - w - 10
        y = h + 10

        cv2.putText(frame, text, (x, y), font, scale, (0, 0, 255), thickness, cv2.LINE_AA)

    def process(self, frame, counter, show_window):
        if not self.process_enable:
            return
        if counter == 0:
            self.current_heart_rate = self.hr_gen.get_sensor() if not self.hr_gen.source else self.hr_gen.get_simulated()

        if show_window:
            self.draw_heart_rate_frame(frame, self.current_heart_rate)
        self.send({"rate": self.current_heart_rate})
