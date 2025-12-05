import cv2
# import keyboard
from speech_to_text import SpeechToText

class MicProcessor:
    def __init__(self):
        self.speech_to_text = SpeechToText()
        self.running = True
        print("Mic Processor initialized.")

    def process(self):
        while self.running:
            self.speech_to_text.process()
            if cv2.waitKey(1) & 0xFF == ord("q"):
                self.running = False
                self.speech_to_text.close()
                break