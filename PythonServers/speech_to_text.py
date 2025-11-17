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




import socket
import json
import sounddevice as sd
from vosk import Model, KaldiRecognizer

class SpeechToText:
    def __init__(self):
        self.UDP_IP = "127.0.0.1"
        self.UDP_PORT = 4244
        self.sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

        self.model = Model("models/vosk-model-small-fr-0.22") # French model but need to add other languages
        self.recognizer = KaldiRecognizer(self.model, 16000)

        self.stream = None

    def process(self):
        print("Microphone listening...")

        with sd.RawInputStream(samplerate=16000, blocksize=8000, dtype="int16", channels=1, callback=self._callback):
            print("Say something!")
            while True:
                sd.sleep(50)

    def _callback(self, indata, frames, time, status):
        if self.recognizer.AcceptWaveform(bytes(indata)):
            result = json.loads(self.recognizer.Result())
            text = result.get("text", "")

            if text:
                print("Text:", text)
                self.sock.sendto(json.dumps({"text": text}).encode(), (self.UDP_IP, self.UDP_PORT))

    def close(self):
        self.sock.close()


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

