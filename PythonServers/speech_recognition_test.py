import speech_recognition as sr

r = sr.Recognizer()
r.pause_threshold = 1.0  # seconds of silence before considering phrase complete
r.energy_threshold = 300 

# Use default microphone as audio source
with sr.Microphone() as source:
    r.adjust_for_ambient_noise(source, duration=1)
    print("Say something...")

    while True: 
        # Listen to the microphone
        # audio_data = r.listen(source, timeout=None, phrase_time_limit=10)
        try:
            audio_data = r.listen(source, timeout=None, phrase_time_limit=10)
            text = r.recognize_google(audio_data, language="fr-FR")
            print(f"{text}")
        except sr.UnknownValueError:
            print("Sorry, I could not understand what you said.")
        except sr.RequestError as e:
            print(f"Could not request results; {e}")




# # pip install SpeechRecognition pyaudio keyboard