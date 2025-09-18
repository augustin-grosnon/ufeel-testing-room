import cv2
from speech_to_text import SpeechToText

class MicProcessor:
    def __init__(self):
        self.speech_to_text = SpeechToText()
        self.running = True

    def process(self):
        while self.running:

            print("here in process of mic processor")
            self.speech_to_text.process()
            if cv2.waitKey(1) & 0xFF == ord("q"):
                self.running = False
                self.speech_to_text.close()
                break
