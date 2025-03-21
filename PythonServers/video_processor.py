import cv2
from emotion_detector import EmotionDetector
from eye_tracker import EyeTracker

class VideoProcessor:
    def __init__(self, show_window=True, capture_id=0):
        self.show_window = show_window
        self.cap = cv2.VideoCapture(capture_id)
        self.emotion_detector = EmotionDetector()
        self.eye_tracker = EyeTracker()
        self.counter = 0
        self.freq = 5

    def process(self):
        while self.cap.isOpened():
            ret, frame = self.cap.read()
            if not ret:
                break
            frame = cv2.flip(frame, 1)
            self.emotion_detector.process(frame, self.counter, show_window=self.show_window)
            self.eye_tracker.process(frame)
            if self.show_window:
                cv2.imshow("Combined Output", frame)
                if cv2.waitKey(1) & 0xFF == ord("q"):
                    break
            self.counter = (self.counter + 1) % self.freq
        self.cap.release()
        if self.show_window:
            cv2.destroyAllWindows()
        self.emotion_detector.close()
        self.eye_tracker.close()
