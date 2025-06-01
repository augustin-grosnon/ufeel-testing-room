using UnityEngine;
using System.Collections;

public class UfeelAPI : MonoBehaviour
{
    private static UfeelAPI? _instance;
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
        Debug.Log("Currently the emotion receiver is: " + (_emotionReceiver != null ? "running" : "shut down"));
        Debug.Log("Currently the eye tracking receiver is: " + (_eyeTrackingReceiver != null ? "running" : "shut down"));
        Debug.Log("-------------------------------------");
    }

    private static void toggleCamera(UDPReceiverBase receiver, bool status)
    {
        return;
        // var json = $"{{\"camera\": {status.ToString().ToLower()}}}";
        // byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
        // receiver.SendData(bytes);
    }

    private static EmotionReceiver? _emotionReceiver = null;

    // --- Emotion Detection Methods ---
    public static void enableCameraEmotion()
    {
        if (_emotionReceiver == null)
        {
            Debug.LogWarning("Can't enable camera if it never start.");
            return;
        }
        toggleCamera(_emotionReceiver, true);
    }

    public static void disableCameraEmotion()
    {
        if (_emotionReceiver == null)
        {
            Debug.LogWarning("Can't disable camera if it never start.");
            return;
        }
        toggleCamera(_emotionReceiver, false);
    }

    private static void toggleEmotionDetection(bool status)
    {
        return;
        // var json = $"{{\"emotion_detection\": {status.ToString().ToLower()}}}";
        // byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
        // _emotionReceiver?.SendData(bytes);
    }

    public void StartEmotionDetection()
    {
        _emotionReceiver = new EmotionReceiver(4243);

        toggleEmotionDetection(true);

        Debug.Log("Emotion detection started.");
    }

    public void StopEmotionDetection()
    {
        if (_emotionReceiver == null)
        {
            Debug.LogWarning("Can't stop if it never start.");
            return;
        }
        toggleEmotionDetection(false);
        _emotionReceiver = null;
        Debug.Log("Emotion detection stopped.");
    }

    public EmotionData? GetCurrentEmotions()
    {
        if (_emotionReceiver == null)
        {
            Debug.LogWarning("Can't get current Emotions if the Emotion detection is not started");
            return null;
        }
        EmotionData? currentEmotions = _emotionReceiver._currentEmotions;
        return currentEmotions;
    }

    public EmotionData.EmotionType? GetDominantEmotion()
    {
        if (_emotionReceiver == null)
        {
            Debug.LogWarning("Can't get dominant Emotion if the Emotion detection is not started");
            return null;
        }
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

    private static EyeTrackingReceiver? _eyeTrackingReceiver = null;

    // --- Eye Tracking Detection Methods ---
     public static void enableCameraEyeTracking()
    {
        if (_eyeTrackingReceiver == null)
        {
            Debug.LogWarning("Can't enable camera if it never start.");
            return;
        }
        toggleCamera(_eyeTrackingReceiver, true);
    }

    public static void disableCameraEyeTracking()
    {
        if (_eyeTrackingReceiver == null)
        {
            Debug.LogWarning("Can't disable camera if it never start.");
            return;
        }
        toggleCamera(_eyeTrackingReceiver, false);
    }

    private static void toggleEyeTrackingDetection(bool status)
    {
        return;
        // var json = $"{{\"eye_tracking_detection\": {status.ToString().ToLower()}}}";
        // byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
        // _eyeTrackingReceiver?.SendData(bytes);
    }

    public void StartEyeTrackingDetection()
    {
        _eyeTrackingReceiver = new EyeTrackingReceiver(4242);

        toggleEyeTrackingDetection(true);

        Debug.Log("Eye Tracking detection started.");
    }

    public void StopEyeTrackingDetection()
    {
        if (_eyeTrackingReceiver == null)
        {
            Debug.LogWarning("Can't stop if it never start.");
            return;
        }
        toggleEyeTrackingDetection(true);
        _eyeTrackingReceiver = null;
        Debug.Log("Eye Tracking detection stopped.");
    }

    public EyeTrackingData? GetCurrentDirections()
    {
        if (_eyeTrackingReceiver == null)
        {
            Debug.LogWarning("Can't get current Eye Tracking if the Eye Tracking detection is not started");
            return null;
        }
        EyeTrackingData? currentEyeTracking = _eyeTrackingReceiver.CurrentEyeTrackingData;
        return currentEyeTracking;
    }

    public EyeTrackingData.EyeTrackingType? GetDominantDirection()
    {
        if (_eyeTrackingReceiver == null)
        {
            Debug.LogWarning("Can't get dominant Eye Tracking if the Eye Tracking detection is not started");
            return null;
        }
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
        if (_emotionReceiver != null)
        {
            toggleEmotionDetection(false);
        }
        if (_eyeTrackingReceiver != null)
        {
            toggleEyeTrackingDetection(false);
        }
        _emotionReceiver = null;
        _eyeTrackingReceiver = null;
        PythonServerController.Instance.StopServer();
    }
}