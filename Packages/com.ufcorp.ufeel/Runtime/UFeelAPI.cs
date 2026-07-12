using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace UFeel
{
    public class UFeelAPI : MonoBehaviour
    {
        private static UFeelAPI _instance;

        private static readonly EmotionReceiver _emotionReceiver = new(4100);
        private static bool _emotionIsRunning;

        private static readonly EyeTrackingReceiver _eyeTrackingReceiver = new(4000);
        private static bool _eyeTrackingIsRunning;

        private static readonly SpeechToTextReceiver _speechToTextReceiver = new(3900);
        private static bool _speechToTextIsRunning;

        private static readonly HeartRateSensorReceiver _heartRateSensorReceiver = new(3800);
        private static bool _heartRateSensorIsRunning;

        private static int _nextRuleId;
        private static readonly List<Rule> _rules = new();
        private static readonly List<Rule> _rulesToAdd = new();
        private static readonly HashSet<int> _rulesToRemove = new();

        private static bool pendingEmotionReceiverStartup;
        private static bool pendingEyeTrackingReceiverStartup;
        private static bool pendingSpeechToTextReceiverStartup;
        private static bool pendingHeartRateReceiverStartup;
        // ! could be cleaner but this will be removed when switching to using a compiled library anyway

        // TODO: Remove this when removing the server
        public static async Task StartAPI()
        {
            if (_instance != null)
                return;

            _instance = FindAnyObjectByType<UFeelAPI>();

            if (_instance != null)
                return;

            GameObject obj = new("UFeelAPI");
            _instance = obj.AddComponent<UFeelAPI>();

            await Task.Delay(millisecondsDelay: 5000);
            Debug.Log("Hello UFEEL User ");

            DontDestroyOnLoad(obj);
        }

// * ------------------------ Rules logic ------------------------ * //
        private void Update()
        {
            SendPendingActivationCommands();

            foreach (Rule rule in _rules)
            {
                if (!rule.Condition())
                    continue;

                rule.Action?.Invoke();

                if (rule.IsUnique)
                    _rulesToRemove.Add(rule.Id);
            }

            if (_rulesToRemove.Count > 0)
            {
                for (int i = _rules.Count - 1; i >= 0; i--)
                {
                    if (_rulesToRemove.Remove(_rules[i].Id))
                    {
                        _rules.RemoveAt(i);
                    }
                }
                _rulesToRemove.Clear();
            }

            if (_rulesToAdd.Count > 0)
            {
                _rules.AddRange(_rulesToAdd);
                _rulesToAdd.Clear();
            }

            RefreshGUI();
        }

        private static void SendPendingActivationCommands()
        {
            if (pendingEmotionReceiverStartup && _emotionReceiver.ClientConnected)
            {
                StartEmotionDetection();
                pendingEmotionReceiverStartup = false;
            }

            if (pendingEyeTrackingReceiverStartup && _eyeTrackingReceiver.ClientConnected)
            {
                StartEyeTrackingDetection();
                pendingEyeTrackingReceiverStartup = false;
            }

            if (pendingHeartRateReceiverStartup && _heartRateSensorReceiver.ClientConnected)
            {
                StartHeartRateDetection();
                pendingHeartRateReceiverStartup = false;
            }

            if (pendingSpeechToTextReceiverStartup && _speechToTextReceiver.ClientConnected)
            {
                StartSpeechDetection();
                pendingSpeechToTextReceiverStartup = false;
            }
        }

        private static void RefreshGUI()
        {
            if (!UFeelDebugHUD.DEBUG_MODE || !UFeelDebugHUD.UseDefaultDebugHUD)
                return;

            UFeelDebugHUD.Set("Emotions", () =>
                _emotionIsRunning ? _emotionReceiver?.CurrentEmotionsData?.ToString() : "Disabled");

            UFeelDebugHUD.Set("Eye Tracking", () =>
                !_eyeTrackingIsRunning ? "Disabled" : _eyeTrackingReceiver.CurrentEyeTrackingData?.ToString());

            UFeelDebugHUD.Set("Speech To Text", () =>
                !_speechToTextIsRunning ? "Disabled" : _speechToTextReceiver.CurrentSpeechToTextData?.text);

            UFeelDebugHUD.Set("Heart Rate Sensor", () =>
                !_heartRateSensorIsRunning ? "Disabled" : _heartRateSensorReceiver.CurrentHeartRateSensorData?.rate.ToString());
        }

        private static RuleKey AddRule(Func<bool> condition, Action action, bool isUnique = false)
        {
            Rule rule = new(
                id: _nextRuleId++,
                condition: condition,
                action: action,
                isUnique: isUnique
            );

            _rulesToAdd.Add(rule);
            return new RuleKey(rule.Id);
        }

        public static void RemoveRule(RuleKey key)
        {
            _rulesToRemove.Add(key.Id);
        }

        public static void Status()
        {
            Debug.Log("-------------------------------------");
            Debug.Log("Currently the emotion receiver is: " + (_emotionIsRunning ? "running" : "shut down"));
            Debug.Log("Currently the eye tracking receiver is: " + (_eyeTrackingIsRunning ? "running" : "shut down"));
            Debug.Log("Currently the speech to text receiver is: " + (_speechToTextIsRunning ? "running" : "shut down"));
            Debug.Log("Currently the heart rate receiver is: " + (_heartRateSensorIsRunning ? "running" : "shut down"));
            Debug.Log("-------------------------------------");
        }

// * ------------------------ EMOTIONS ------------------------ * //
        private static void ToggleEmotionDetection(bool status)
        {
            byte[] bytes = ClientBase.CreateData("emotion_detection", status.ToString().ToLower());
            _emotionReceiver?.SendData(bytes);
        }

        public static void StartEmotionDetection()
        {
            if (!_emotionReceiver.ClientConnected)
            {
                pendingEmotionReceiverStartup = true;
                return;
            }

            ToggleEmotionDetection(true);
            _emotionIsRunning = true;
            Debug.Log("Emotion detection started.");
        }

        public static void StopEmotionDetection()
        {
            ToggleEmotionDetection(false);
            _emotionIsRunning = false;
            _emotionReceiver.ResetData();
            Debug.Log("Emotion detection stopped.");
        }

        public static EmotionData? CurrentEmotionsData
        {
            get
            {
                if (!_emotionIsRunning) return null;

                return _emotionReceiver.CurrentEmotionsData;
            }
        }

        public static EmotionData.EmotionType? DominantEmotion
        {
            get
            {
                if (!_emotionIsRunning) return null;

                EmotionData? currentEmotions = _emotionReceiver.CurrentEmotionsData;
                return currentEmotions?.DominantEmotion;
            }
        }

        public static RuleKey TriggerActionOnEmotion(EmotionData.EmotionType emotion, Action action, bool isUnique)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            return AddRule(
                condition: () => DominantEmotion == emotion,
                action: action,
                isUnique: isUnique
            );
        }

        public static RuleKey TriggerActionOnEmotionOnce(EmotionData.EmotionType emotion, Action action)
        {
            return TriggerActionOnEmotion(emotion, action, true);
        }

        public static RuleKey TriggerActionOnEmotionContinuous(EmotionData.EmotionType emotion, Action action)
        {
            return TriggerActionOnEmotion(emotion, action, false);
        }

// * ------------------------ EYE TRACKING ------------------------ * //
        private static void ToggleEyeTrackingDetection(bool status)
        {
            byte[] bytes = ClientBase.CreateData("eye_detection", status.ToString().ToLower());
            _eyeTrackingReceiver?.SendData(bytes);
        }

        public static void StartEyeTrackingDetection() // ? should we check if it is already running to avoid sending a start command to an already started module? or it doesn't matter
        {
            if (!_eyeTrackingReceiver.ClientConnected)
            {
                pendingEyeTrackingReceiverStartup = true;
                return;
            }

            ToggleEyeTrackingDetection(true);
            _eyeTrackingIsRunning = true;
            Debug.Log("Eye Tracking detection started.");
        }

        public static void StopEyeTrackingDetection()
        {
            ToggleEyeTrackingDetection(false);
            _eyeTrackingIsRunning = false;
            _eyeTrackingReceiver.ResetData();
            Debug.Log("Eye Tracking detection stopped.");
        }

        public static EyeTrackingData? CurrentDirections
        {
            get
            {
                if (!_eyeTrackingIsRunning) return null;

                return _eyeTrackingReceiver.CurrentEyeTrackingData;
            }
        }

        public static EyeTrackingData.EyeTrackingDirection? DominantDirection
        {
            get
            {
                if (!_eyeTrackingIsRunning) return null;

                EyeTrackingData? currentEyeTracking = _eyeTrackingReceiver.CurrentEyeTrackingData;
                return currentEyeTracking?.CurrentEyeTrackingDirection;
            }
        }

        public static bool? BlinkStatus
        {
            get
            {
                if (!_eyeTrackingIsRunning) return null;

                return _eyeTrackingReceiver.CurrentEyeTrackingData?.blink;
            }
        }

        public static RuleKey TriggerActionOnDirection(EyeTrackingData.EyeTrackingDirection direction, Action action, bool isUnique)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            return AddRule(
                condition: () => DominantDirection == direction,
                action: action,
                isUnique: isUnique
            );
        }

        public static RuleKey TriggerActionOnDirectionOnce(EyeTrackingData.EyeTrackingDirection direction, Action action)
        {
            return TriggerActionOnDirection(direction, action, true);
        }

        public static RuleKey TriggerActionOnDirectionContinuous(EyeTrackingData.EyeTrackingDirection direction, Action action)
        {
            return TriggerActionOnDirection(direction, action, false);
        }

// * ------------------------ SPEECH TO TEXT ------------------------ * //
        private static void ToggleSpeechDetection(bool status)
        {
            byte[] bytes = ClientBase.CreateData("speech_detection", status.ToString().ToLower());
            _speechToTextReceiver?.SendData(bytes);
        }

        public static void StartSpeechDetection()
        {
            if (!_speechToTextReceiver.ClientConnected)
            {
                pendingSpeechToTextReceiverStartup = true;
                return;
            }

            ToggleSpeechDetection(true);
            _speechToTextIsRunning = true;
            Debug.Log("Speech detection started.");
        }

        public static void StopSpeechDetection()
        {
            ToggleSpeechDetection(false);
            _speechToTextIsRunning = false;
            _speechToTextReceiver.ResetData();
            Debug.Log("Speech detection stopped.");
        }

        public static string CurrentSpeech
        {
            get
            {
                if (!_speechToTextIsRunning) return null;

                return _speechToTextReceiver.CurrentSpeechToTextData?.text;
            }
        }

        public static RuleKey TriggerActionOnSpeech(string text, Action action, bool isUnique)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            return AddRule(
                condition: () =>
                {
                    SpeechToTextData? currentSpeechData = _speechToTextReceiver.CurrentSpeechToTextData;
                    if (currentSpeechData == null) return false;

                    string targetToLower = text.ToLower();
                    string currentToLower = currentSpeechData?.text.ToLower();
                    return targetToLower.Contains(currentToLower);
                },
                action: action,
                isUnique: isUnique
            );
        }

        public static RuleKey TriggerActionOnSpeechOnce(string text, Action action)
        {
            return TriggerActionOnSpeech(text, action, true);
        }

        public static RuleKey TriggerActionOnSpeechContinuous(string text, Action action)
        {
            return TriggerActionOnSpeech(text, action, false);
        }

// * ------------------------ Heart Rate ------------------------ * //
        private static void ToggleHeartRateDetection(bool status)
        {
            byte[] bytes = ClientBase.CreateData("heart_rate_detection", status.ToString().ToLower());
            _heartRateSensorReceiver?.SendData(bytes);
        }

        public static void StartHeartRateDetection()
        {
            if (!_heartRateSensorReceiver.ClientConnected)
            {
                pendingHeartRateReceiverStartup = true;
                return;
            }

            ToggleHeartRateDetection(true);
            _heartRateSensorIsRunning = true;
            Debug.Log("Heart Rate detection started.");
        }

        public static void StopHeartRateDetection()
        {
            ToggleHeartRateDetection(false);
            _heartRateSensorIsRunning = false;
            _heartRateSensorReceiver.ResetData();
            Debug.Log("Heart Rate detection stopped.");
        }

        public static int? CurrentHeartRate
        {
            get
            {
                if (!_heartRateSensorIsRunning) return 0;

                HeartRateSensorData? currentHeartRateSensorData = _heartRateSensorReceiver.CurrentHeartRateSensorData;
                return currentHeartRateSensorData?.rate;
            }
        }

        public static RuleKey TriggerActionOnHeartRate(int rate, Action action, bool isUnique, int tolerance = 0)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            return AddRule(
                condition: () =>
                {
                    HeartRateSensorData? currentHeartRateSensorData = _heartRateSensorReceiver.CurrentHeartRateSensorData;

                    if (!currentHeartRateSensorData.HasValue)
                        return false;

                    int current = currentHeartRateSensorData.Value.rate;

                    int min = rate - tolerance;
                    int max = rate + tolerance;

                    return current >= min && current <= max;
                },
                action: action,
                isUnique: isUnique
            );
        }

        public static RuleKey TriggerActionOnHeartRateOnce(int rate, Action action, int tolerance = 0)
        {
            return TriggerActionOnHeartRate(rate, action, true, tolerance);
        }

        public static RuleKey TriggerActionOnDirectionContinuous(int rate, Action action, int tolerance = 0)
        {
            return TriggerActionOnHeartRate(rate, action, false, tolerance);
        }

// * ------------------------ Stop method ------------------------ * //
        private void OnDisable()
        {
            StopAPI();
            Debug.Log("Game stopped - OnDisable called!");
        }

        public static void ToggleOffEverything()
        {
            StopEmotionDetection();
            StopEyeTrackingDetection();
            StopSpeechDetection();
            StopHeartRateDetection();
        }

        public static void StopAPI()
        {
            ToggleOffEverything();

            PythonServerController.Instance.StopServer();
        }
    }
}
