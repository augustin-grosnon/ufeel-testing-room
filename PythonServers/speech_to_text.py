import speech_recognition as sr
import socket
import json

class SpeechToText:
    def __init__(self):
        self.UDP_IP = "127.0.0.1"
        self.UDP_PORT = 4244
        self.sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        self.recognizer = sr.Recognizer()
        self.language = "fr-FR"
        self.recognizer.pause_threshold = 0.5
        self.recognizer.energy_threshold = 300

    def process(self, phrase_time_limit=0.5):
        with sr.Microphone() as source:
            self.recognizer.adjust_for_ambient_noise(source, duration=1)
            print("Say something...")

            try:
                audio_data = self.recognizer.listen(source, timeout=None, phrase_time_limit=phrase_time_limit)
                text = self.recognizer.recognize_google(audio_data, language=self.language)
                self.sock.sendto(json.dumps({"text": text}).encode(), (self.UDP_IP, self.UDP_PORT))
                print(f"the text is: {text}")
            except sr.UnknownValueError:
                print("Could not understand.")
            except sr.RequestError as e:
                print(f"Could not request results; {e}")

    def close(self):
        self.sock.close()

 # pip install SpeechRecognition pyaudio keyboard -> put in requirement file