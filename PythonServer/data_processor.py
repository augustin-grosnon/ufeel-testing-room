import os

import cv2
import time
from emotion_detector import EmotionDetector
from eye_tracker import EyeTracker
from speech_to_text import SpeechToText
from heart_rate_sensor import HeartRateSensor

class DataProcessor:
    def __init__(self, calibration, show_window=True, capture_id=0):
        self.show_window = show_window
        self.cap = cv2.VideoCapture(capture_id)
        # if not calibration:

        base_dir = os.path.dirname(os.path.abspath(__file__))
        emotion_model_path = os.path.join(base_dir, 'models', 'emotion_detection','binary_affectnet_resnet34_pretrained.pt')
        emotion_config_path = os.path.join(base_dir, 'models', 'emotion_detection', 'binary_affectnet_resnet34_pretrained.yaml')
        self.emotion_detector = EmotionDetector(
            model_path=emotion_model_path,
            config_path=emotion_config_path,
        )

        self.eye_tracker = EyeTracker()
        self.speech_to_text = SpeechToText()
        self.heart_rate_sensor = HeartRateSensor()

        self.emotion_counter = 0
        self.emotion_freq = 5

        self.heartbeat_counter = 0
        self.heartbeat_freq = 60

        self.scale_factor = 1.3
        self.calibration = calibration

        self.target_hz = 30
        self.frame_time = 1.0 / self.target_hz

    def process(self):
        while self.cap.isOpened():
            start_time = time.time()
            ret, frame = self.cap.read()
            if not ret:
                break
            frame = cv2.flip(frame, 1)
            #if not self.calibration:
            self.emotion_detector.process(frame, self.emotion_counter, show_window=self.show_window)
            self.eye_tracker.process(frame, self.calibration, show_window=self.show_window)
            self.speech_to_text.process(frame, self.show_window)
            self.heart_rate_sensor.process(frame, self.heartbeat_counter, self.show_window)

            if self.show_window:
                resized_frame = cv2.resize(
                    frame,
                    None,
                    fx=self.scale_factor,
                    fy=self.scale_factor,
                    interpolation=cv2.INTER_LINEAR
                )
                cv2.imshow("Combined Output", resized_frame)
                if cv2.waitKey(1) & 0xFF == ord("p"):
                    break
            self.emotion_counter = (self.emotion_counter + 1) % self.emotion_freq
            self.heartbeat_counter = (self.heartbeat_counter + 1) % self.heartbeat_freq

            elapsed = time.time() - start_time
            sleep_time = self.frame_time - elapsed

            if sleep_time > 0:
                time.sleep(sleep_time)

        self.cap.release()
        if self.show_window:
            cv2.destroyAllWindows()
        self.emotion_detector.close()
        self.eye_tracker.close()
        self.speech_to_text.close()
        self.heart_rate_sensor.close()
