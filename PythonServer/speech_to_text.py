# import speech_recognition as sr
# import socket
# import json

# class SpeechToText:
#     def __init__(self):
#         self.UDP_IP = "127.0.0.1"
#         self.UDP_PORT = 4244
#         self.sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
#         self.recognizer = sr.Recognizer()
#         self.language = "fr-FR"
#         self.recognizer.pause_threshold = 0.5
#         self.recognizer.energy_threshold = 300
#         self._adjusted = False




#     def process(self, phrase_time_limit=2):
#         while True:
#             self.sock.sendto(json.dumps({"text": "Bonjour"}).encode(), (self.UDP_IP, self.UDP_PORT))
#             print("Sent Bonjour")
#         # with sr.Microphone() as source:
#         #     self.recognizer.adjust_for_ambient_noise(source, duration=1)
#         #     print("Say something...")

#         #     while True:
#         #         self.sock.sendto(json.dumps({"text": "Bonjour"}).encode(), (self.UDP_IP, self.UDP_PORT))

#                 # try:
#                 #     audio_data = self.recognizer.listen(source, timeout=None, phrase_time_limit=phrase_time_limit)
#                 #     text = self.recognizer.recognize_google(audio_data, language=self.language)
#                 #     self.sock.sendto(json.dumps({"text": text}).encode(), (self.UDP_IP, self.UDP_PORT))
#                 #     print(f"the text is: {text}")
#                 # except sr.UnknownValueError:
#                 #     print("Could not understand.")
#                 # except sr.RequestError as e:
#                 #     print(f"Could not request results; {e}")

#     def close(self):
#         self.sock.close()

# # pip install SpeechRecognition pyaudio keyboard -> put in requirement file

# # recognize_google() API Google
# # try Vosk (~50–200 Mo), Whisper (more accurate but lourd, 150 Mo à 3 Go)


import cv2
import json
import sounddevice as sd
from vosk import Model, KaldiRecognizer
from client_base import ClientBase
import logging
import threading
import os

logging.basicConfig(
    filename="client_base.log",
    filemode="a",  # Append mode
    format="%(asctime)s - %(levelname)s - %(message)s",
    level=logging.DEBUG  # Use DEBUG level to log everything
)

class SpeechToText(ClientBase):
    def __init__(self, model_path=None):
        super().__init__("127.0.0.1", 3900)
        self.handlers = {
            "speech_detection": self.toggle_speech_detection,
        }

        base_dir = os.path.dirname(os.path.abspath(__file__))
        if model_path is None:
            model_path = os.path.join(base_dir, 'models', 'vosk-model-small-fr-0.22')
        self.model = Model(model_path) # French model but need to add other languages
        self.recognizer = KaldiRecognizer(self.model, 16000)

        self.process_enable = False
        self.current_text = "None"
        self.thread = None

    def toggle_speech_detection(self, state):
        self.process_enable = state
        status = "enabled" if state else "disabled"
        logging.info(f"Speech detection {status} {state}")

    def _start_thread(self):
        if self.thread is not None:
            return
        self.thread = threading.Thread(target=self._run_audio_loop, daemon=True)
        self.thread.start()

    def _stop_thread(self):
        if self.thread is not None:
            self.thread.join(timeout=1)
        self.thread = None

    def draw_centered_text_bottom(self, frame, text, max_width_ratio=0.8, line_height=40):
        h, w, _ = frame.shape
        max_width = int(w * max_width_ratio)

        words = text.split()
        lines = []
        current_line = ""

        for word in words:
            test_line = current_line + (" " if current_line else "") + word
            text_size = cv2.getTextSize(test_line, cv2.FONT_HERSHEY_SIMPLEX, 0.8, 2)[0]

            if text_size[0] <= max_width:
                current_line = test_line
            else:
                lines.append(current_line)
                current_line = word

        if current_line:
            lines.append(current_line)

        total_height = len(lines) * line_height
        y_start = h - total_height - 20

        for i, line in enumerate(lines):
            text_size = cv2.getTextSize(line, cv2.FONT_HERSHEY_SIMPLEX, 0.8, 2)[0]
            x = (w - text_size[0]) // 2
            y = y_start + i * line_height
            cv2.putText(frame, line, (x, y), cv2.FONT_HERSHEY_SIMPLEX, 0.8, (0, 0, 255), 2)

    def process(self, frame):
        if self.process_enable:
            self._start_thread()
            self.draw_centered_text_bottom(frame, self.current_text)
        else:
            self._stop_thread()

    def _run_audio_loop(self):
        try:
            with sd.RawInputStream(samplerate=16000, blocksize=8000, dtype="int16", channels=1, callback=self._callback):
                while True:
                    sd.sleep(5)
        except Exception as e:
            print("Audio thread error:", e)


    def _callback(self, indata, frames, time, status):
        if self.recognizer.AcceptWaveform(bytes(indata)):
            result = json.loads(self.recognizer.Result())
            text = result.get("text", "")

            if text:
                print("Text:", text)
                self.current_text = text
                self.send({"text": text})


#pip install vosk sounddevice
# add other languages: https://alphacephei.com/vosk/models





# changement du model speech recognition vers Vosk car speech recognition etait une api google
# qui avait besoin d'internet pour fonctionner
# un peu plus lent par rapport a Vosk
# un peu du mal a detecter

#Vosk c'est un model rapide, offline
# accurate
# on peut également ajouter des models un peu plus lourds pour plus de précision
# j'ai prit le model francais small (41 Mo) -> l'autre model francais normal fait 1.4 Go
