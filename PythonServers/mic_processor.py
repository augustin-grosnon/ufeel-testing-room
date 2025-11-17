import cv2
# import keyboard
from speech_to_text import SpeechToText

class MicProcessor:
    def __init__(self):
        self.speech_to_text = SpeechToText()
        self.running = True
        print("Mic Processor initialized.")

    def process(self):
        print("Starting Mic Processor...")
        while self.running:
            print("here in process of mic processor")
            self.speech_to_text.process()
            if cv2.waitKey(1) & 0xFF == ord("q"):
            # if keyboard.is_pressed('q'):
                self.running = False
                self.speech_to_text.close()
                break
