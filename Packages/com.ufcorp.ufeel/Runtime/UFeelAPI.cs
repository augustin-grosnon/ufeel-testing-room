using UnityEngine;
using System;
using System.Collections.Generic;

namespace UFeel
{
    public class UFeelAPI : MonoBehaviour
    {
        private static UFeelAPI _instance;

        private readonly static EmotionReceiver _emotionReceiver = new(4100);
        private static bool _emotionIsRunning = false;

        private readonly static EyeTrackingReceiver _eyeTrackingReceiver = new(4000);
        private static bool _eyeTrackingIsRunning = false;

        private readonly static SpeechToTextReceiver _speechReceiver = new(3900);
        private static bool _speechIsRunning = false;

        private readonly static HeartRateSensorReceiver _heartRateReceiver = new(3800);
        private static bool _heartRateIsRunning = false;
        private int _nextRuleId = 0;

        private readonly List<Rule> _rules = new();
        private readonly List<Rule> _rulesToAdd = new();
        private readonly HashSet<int> _rulesToRemove = new();

        public static UFeelAPI Instance
        {
            get
            {
                if (_instance == null)
                {
                     _instance = FindAnyObjectByType<UFeelAPI>();

                    if (_instance == null)
                    {
                        GameObject obj = new("UFeelAPI");
                        _instance = obj.AddComponent<UFeelAPI>();

                        DontDestroyOnLoad(obj);
                    }
                }
                return _instance;
            }
        }

// * ------------------------ Rules logic ------------------------ * //
        private void Update()
        {
            foreach (var rule in _rules)
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
        }

        private RuleKey AddRule(Func<bool> condition, Action action, bool isUnique = false)
        {
            var rule = new Rule(
                id: _nextRuleId++,
                condition: condition,
                action: action,
                isUnique: isUnique
            );

            _rulesToAdd.Add(rule);
            return new RuleKey(rule.Id);
        }

        public void RemoveRule(RuleKey key)
        {
            _rulesToRemove.Add(key.Id);
        }

        public void Status()
        {
            Debug.Log("-------------------------------------");
            Debug.Log("Currently the emotion receiver is: " + (_emotionIsRunning ? "running" : "shut down"));
            Debug.Log("Currently the eye tracking receiver is: " + (_eyeTrackingIsRunning ? "running" : "shut down"));
            Debug.Log("Currently the speech to text receiver is: " + (_speechIsRunning ? "running" : "shut down"));
            Debug.Log("Currently the heart rate receiver is: " + (_heartRateIsRunning ? "running" : "shut down"));
            Debug.Log("-------------------------------------");
        }

// * ------------------------ EMOTIONS ------------------------ * //
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

        public RuleKey TriggerActionOnEmotion(EmotionData.EmotionType emotion, Action action, bool isUnique)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            return AddRule(
                condition: () => GetDominantEmotion() == emotion,
                action: action,
                isUnique: isUnique
            );
        }

        public RuleKey TriggerActionOnEmotionOnce(EmotionData.EmotionType emotion, Action action)
        {
            return TriggerActionOnEmotion(emotion, action, true);
        }

        public RuleKey TriggerActionOnEmotionContinuous(EmotionData.EmotionType emotion, Action action)
        {
            return TriggerActionOnEmotion(emotion, action, false);
        }

// * ------------------------ EYE TRACKING ------------------------ * //
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
            ToggleEyeTrackingDetection(false);
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

        public RuleKey TriggerActionOnDirection(EyeTrackingData.EyeTrackingType direction, Action action, bool isUnique)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            return AddRule(
                condition: () => GetDominantDirection() == direction,
                action: action,
                isUnique: isUnique
            );
        }

        public RuleKey TriggerActionOnDirectionOnce(EyeTrackingData.EyeTrackingType direction, Action action)
        {
            return TriggerActionOnDirection(direction, action, true);
        }

        public RuleKey TriggerActionOnDirectionContinuous(EyeTrackingData.EyeTrackingType direction, Action action)
        {
            return TriggerActionOnDirection(direction, action, false);
        }

// * ------------------------ SPEECH TO TEXT ------------------------ * //
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

        public string GetCurrentSpeech()
        {
            if (!_speechIsRunning) return null;

            SpeechToTextData? currentSpeechData = _speechReceiver.CurrentSpeechData;
            return currentSpeechData?.text;
        }

        public RuleKey TriggerActionOnSpeech(string text, Action action, bool isUnique)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            return AddRule(
                condition: () =>
                {
                    SpeechToTextData? currentSpeechData = _speechReceiver.CurrentSpeechData;
                    if (currentSpeechData == null) return false;

                    string targetToLower = text.ToLower();
                    string currentToLower = currentSpeechData?.text.ToLower();
                    return targetToLower.Contains(currentToLower);
                },
                action: action,
                isUnique: isUnique
            );
        }

        public RuleKey TriggerActionOnSpeechOnce(string text, Action action)
        {
            return TriggerActionOnSpeech(text, action, true);
        }

        public RuleKey TriggerActionOnSpeechContinuous(string text, Action action)
        {
            return TriggerActionOnSpeech(text, action, false);
        }

// * ------------------------ Heart Rate ------------------------ * //
        private static void ToggleHeartRateDetection(bool status)
        {
            byte[] bytes = ClientBase.CreateData("heart_rate_detection", status.ToString().ToLower());
            _heartRateReceiver?.SendData(bytes);
        }

        public void StartHeartRateDetection()
        {
            ToggleHeartRateDetection(true);
            _heartRateIsRunning = true;
            Debug.Log("Heart Rate detection started.");
            Debug.Log("toi là");
        }

        public void StopHeartRateDetection()
        {
            ToggleHeartRateDetection(false);
            _heartRateIsRunning = false;
            Debug.Log("Heart Rate detection stopped.");
        }

        public int? GetCurrentHeartRate()
        {
            if (!_heartRateIsRunning) return 0;

            HeartRateSensorData? currentHeartRateSensorData = _heartRateReceiver.CurrentHeartRateSensorData;
            return currentHeartRateSensorData?.rate;
        }

        public RuleKey TriggerActionOnHeartRate(int rate, Action action, bool isUnique, int tolerance = 0)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            return AddRule(
                condition: () =>
                {
                    HeartRateSensorData? currentHeartRateSensorData = _heartRateReceiver.CurrentHeartRateSensorData;

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

        public RuleKey TriggerActionOnHeartRateOnce(int rate, Action action, int tolerance = 0)
        {
            return TriggerActionOnHeartRate(rate, action, true, tolerance);
        }

        public RuleKey TriggerActionOnDirectionContinuous(int rate, Action action, int tolerance = 0)
        {
            return TriggerActionOnHeartRate(rate, action, false, tolerance);
        }

// * ------------------------ Stop method ------------------------ * //
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
            ToggleHeartRateDetection(false);

            PythonServerController.Instance.StopServer();
        }
    }
}