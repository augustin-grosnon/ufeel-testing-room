import sys

print(sys.executable)
print(sys.path)

import cv2
from emotion_detector import EmotionDetector
from eye_tracker import EyeTracker
from speech_to_text import SpeechToText

class DataProcessor:
    def __init__(self, calibration, show_window=True, capture_id=0):
        self.show_window = show_window
        self.cap = cv2.VideoCapture(capture_id)
        # if not calibration:
        self.emotion_detector = EmotionDetector()
        self.eye_tracker = EyeTracker()
        self.speech_to_text = SpeechToText()
        self.counter = 0
        self.freq = 5
        self.scale_factor = 1.3
        self.calibration = calibration

    def process(self):
        while self.cap.isOpened():
            ret, frame = self.cap.read()
            if not ret:
                break
            frame = cv2.flip(frame, 1)
            #if not self.calibration:
            self.emotion_detector.process(frame, self.counter, show_window=self.show_window)
            self.eye_tracker.process(frame, self.calibration, show_window=self.show_window)
            self.speech_to_text.process(frame)
            if self.show_window:
                resized_frame = cv2.resize(
                    frame,
                    None,
                    fx=self.scale_factor,
                    fy=self.scale_factor,
                    interpolation=cv2.INTER_LINEAR
                )
                cv2.imshow("Combined Output", resized_frame)
                if cv2.waitKey(1) & 0xFF == ord("q"):
                    break
            self.counter = (self.counter + 1) % self.freq
        self.cap.release()
        if self.show_window:
            cv2.destroyAllWindows()
        self.emotion_detector.close()
        self.eye_tracker.close()
        self.speech_to_text.close()
