using UnityEngine;
using System.Collections;
using System.Text;

public class UfeelAPI : MonoBehaviour
{
    private static UfeelAPI? _instance;
    private static EmotionReceiver _emotionReceiver = new EmotionReceiver(4102);
    private static bool _emotionIsRunning = false;
    private static EyeTrackingReceiver _eyeTrackingReceiver = new EyeTrackingReceiver(4002);
    private static bool _eyeTrackingIsRunning = false;

    public static UfeelAPI Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new UfeelAPI();

            }
            return _instance;
        }
    }

    public void status()
    {
        Debug.Log("-------------------------------------");
        Debug.Log("Currently the emotion receiver is: " + (_emotionIsRunning ? "running" : "shut down"));
        Debug.Log("Currently the eye tracking receiver is: " + (_eyeTrackingIsRunning ? "running" : "shut down"));
        Debug.Log("-------------------------------------");
    }

    private static void toggleCamera(UDPReceiverBase receiver, bool status)
    {
        var json = $"{{\"camera\": {status.ToString().ToLower()}}}";
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
        receiver.SendData(bytes);
    }


    // --- Emotion Detection Methods ---
    public static void enableCameraEmotion()
    {
        toggleCamera(_emotionReceiver, true);
    }

    public static void disableCameraEmotion()
    {
        toggleCamera(_emotionReceiver, false);
    }

    private static void toggleEmotionDetection(bool status)
    {
        string message = status ? "emotion_detection_on" : "emotion_detection_off";
        byte[] bytes = Encoding.UTF8.GetBytes(message);
        _emotionReceiver?.SendData(bytes);
    }

    public void StartEmotionDetection()
    {
        toggleEmotionDetection(true);
        _emotionIsRunning = true;
        Debug.Log("Emotion detection started.");
    }

    public void StopEmotionDetection()
    {
        toggleEmotionDetection(false);
        _emotionIsRunning = false;
        Debug.Log("Emotion detection stopped.");
    }

    public EmotionData? GetCurrentEmotions()
    {
        if (!_emotionIsRunning) return null;

        EmotionData? currentEmotions = _emotionReceiver._currentEmotions;
        return currentEmotions;
    }

    public EmotionData.EmotionType? GetDominantEmotion()
    {
        if (!_emotionIsRunning) return null;

        EmotionData? currentEmotions = _emotionReceiver._currentEmotions;
        return currentEmotions?.GetDominantEmotion();
    }

    public void TriggerActionIfEmotion(EmotionData.EmotionType emotion, System.Action action)
    {
        if (action == null) return;

        if (emotion == GetDominantEmotion())
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
     public static void enableCameraEyeTracking()
    {
        toggleCamera(_eyeTrackingReceiver, true);
    }

    public static void disableCameraEyeTracking()
    {
        toggleCamera(_eyeTrackingReceiver, false);
    }

    private static void toggleEyeTrackingDetection(bool status)
    {
        string message = status ? "eye_tracking_on" : "eye_tracking_off";
        byte[] bytes = Encoding.UTF8.GetBytes(message);
        _eyeTrackingReceiver?.SendData(bytes);
    }

    public void StartEyeTrackingDetection()
    {
        toggleEyeTrackingDetection(true);
        _eyeTrackingIsRunning = true;
        Debug.Log("Eye Tracking detection started.");
    }

    public void StopEyeTrackingDetection()
    {
        toggleEyeTrackingDetection(true);
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

    public void stopAPI()
    {
        toggleEmotionDetection(false);
        toggleEyeTrackingDetection(false);

        PythonServerController.Instance.StopServer();
    }
}