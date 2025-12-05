using UnityEngine;
using System.Collections;
using System.Text;


namespace UFeel
{
    public class UfeelAPI : MonoBehaviour
    {
        private static UfeelAPI _instance;
        private static EmotionReceiver _emotionReceiver = new(4100);
        private static bool _emotionIsRunning = false;
        private static EyeTrackingReceiver _eyeTrackingReceiver = new(4000);
        private static bool _eyeTrackingIsRunning = false;
        private static SpeechToTextReceiver _speechReceiver = new(3900);
        private static bool _speechIsRunning = false;

        public static UfeelAPI Instance
        {
            get
            {
                if (_instance == null)
                {
                     _instance = FindAnyObjectByType<UfeelAPI>();

                    if (_instance == null)
                    {
                        GameObject obj = new("UfeelAPI");
                        _instance = obj.AddComponent<UfeelAPI>();

                        DontDestroyOnLoad(obj);
                    }
                }
                return _instance;
            }
        }

        public void Status()
        {
            Debug.Log("-------------------------------------");
            Debug.Log("Currently the emotion receiver is: " + (_emotionIsRunning ? "running" : "shut down"));
            Debug.Log("Currently the eye tracking receiver is: " + (_eyeTrackingIsRunning ? "running" : "shut down"));
            Debug.Log("Currently the speech to text receiver is: " + (_speechIsRunning ? "running" : "shut down"));
            Debug.Log("-------------------------------------");
        }

        // private static void toggleCamera(ClientBase receiver, bool status)
        // {
        //     var json = $"{{\"camera\": {status.ToString().ToLower()}}}";
        //     byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
        //     receiver.SendData(bytes);
        // }


        // // --- Emotion Detection Methods ---
        // public static void enableCameraEmotion()
        // {
        //     toggleCamera(_emotionReceiver, true);
        // }

        // public static void disableCameraEmotion()
        // {
        //     toggleCamera(_emotionReceiver, false);
        // }

        private static void ToggleEmotionDetection(bool status)
        {
            byte[] bytes = ClientBase.CreateData("emotion_detection", status.ToString().ToLower());
            _emotionReceiver?.SendData(bytes);
        }

        public void StartEmotionDetection()
        {
            ToggleEmotionDetection(true);
            _emotionIsRunning = true;
            Debug.Log("Emotion detection started.");
        }

        public void StopEmotionDetection()
        {
            ToggleEmotionDetection(false);
            _emotionIsRunning = false;
            Debug.Log("Emotion detection stopped.");
        }

        public EmotionData? GetCurrentEmotions()
        {
            if (!_emotionIsRunning) return null;

            EmotionData? currentEmotions = _emotionReceiver.CurrentEmotions;
            return currentEmotions;
        }

        public EmotionData.EmotionType? GetDominantEmotion()
        {
            if (!_emotionIsRunning) return null;

            EmotionData? currentEmotions = _emotionReceiver.CurrentEmotions;
            return currentEmotions?.GetDominantEmotion();
        }

        public void TriggerActionIfEmotion(EmotionData.EmotionType emotion, System.Action action)
        {
            if (action == null) return;

            EmotionData.EmotionType? currentEmotion = GetDominantEmotion();
            if (emotion == currentEmotion)
                action.Invoke();
        }

        // private IEnumerator CheckEmotionRoutine(EmotionData.EmotionType targetEmotion, Action action, float interval)
        // {
        //     while (true)
        //     {
        //         if (GetDominantEmotion() == targetEmotion)
        //         {
        //             Debug.Log($"Dominant emotion is {targetEmotion}, invoking action.");
        //             action?.Invoke();
        //         }

        //         yield return new WaitForSeconds(interval);
        //     }
        // }

        // public Coroutine TriggerActionWhenDominantEmotionIs(EmotionData.EmotionType emotion, Action action, float checkInterval = 0.5f)
        // {
        //     return StartCoroutine(CheckEmotionRoutine(emotion, action, checkInterval));
        // }

        // --- Eye Tracking Detection Methods ---
        //  public static void enableCameraEyeTracking()
        // {
        //     toggleCamera(_eyeTrackingReceiver, true);
        // }

        // public static void disableCameraEyeTracking()
        // {
        //     toggleCamera(_eyeTrackingReceiver, false);
        // }

        private static void ToggleEyeTrackingDetection(bool status)
        {
            byte[] bytes = ClientBase.CreateData("eye_detection", status.ToString().ToLower());

            _eyeTrackingReceiver?.SendData(bytes);
        }

        public void StartEyeTrackingDetection()
        {
            ToggleEyeTrackingDetection(true);
            _eyeTrackingIsRunning = true;
            Debug.Log("Eye Tracking detection started.");
        }

        public void StopEyeTrackingDetection()
        {
            ToggleEyeTrackingDetection(true);
            _eyeTrackingIsRunning = false;
            Debug.Log("Eye Tracking detection stopped.");
        }

        public EyeTrackingData? GetCurrentDirections()
        {
            if (!_eyeTrackingIsRunning) return null;

            EyeTrackingData? currentEyeTracking = _eyeTrackingReceiver.CurrentEyeTrackingData;
            return currentEyeTracking;
        }

        public EyeTrackingData.EyeTrackingType? GetDominantDirection()
        {
            if (!_eyeTrackingIsRunning) return null;

            EyeTrackingData? currentEyeTracking = _eyeTrackingReceiver.CurrentEyeTrackingData;
            return currentEyeTracking?.GetEyeTrackingType();
        }

        public void TriggerActionIfDirection(EyeTrackingData.EyeTrackingType direction, System.Action action)
        {
            if (action == null) return;

            if (direction == GetDominantDirection())
                action.Invoke();
        }

        private static void ToggleSpeechDetection(bool status)
        {
            byte[] bytes = ClientBase.CreateData("speech_detection", status.ToString().ToLower());
            _speechReceiver?.SendData(bytes);
        }

        public void StartSpeechDetection()
        {
            ToggleSpeechDetection(true);
            _speechIsRunning = true;
            Debug.Log("Speech detection started.");
        }

        public void StopSpeechDetection()
        {
            ToggleSpeechDetection(false);
            _speechIsRunning = false;
            Debug.Log("Speech detection stopped.");
        }

        // TODO: is it uf enough ?
        public string GetCurrentSpeech()
        {
            if (!_speechIsRunning) return null;

            SpeechData? currentSpeechData = _speechReceiver.CurrentSpeechData;
            return currentSpeechData?.text;
        }

        public void TriggerActionIfSpeech(string text, System.Action action)
        {
            if (action == null) return;

            SpeechData? currentSpeechData = _speechReceiver.CurrentSpeechData;
            if (currentSpeechData == null) return;

            string targetToLower = text.ToLower();
            string currentToLower = currentSpeechData?.text.ToLower();
            Debug.Log("Current word is " + currentToLower);
            if (targetToLower.Contains(currentToLower))
                action.Invoke();
        }

        private void OnDisable()
        {
            StopAPI();
            Debug.Log("Game stopped — OnDisable called!");
        }

        public void StopAPI()
        {
            ToggleEmotionDetection(false);
            ToggleEyeTrackingDetection(false);
            ToggleSpeechDetection(false);

            PythonServerController.Instance.StopServer();
        }
    }
}