using UFeel;
using UnityEditor;
using UnityEngine;

public class LauncherScript : MonoBehaviour
{
    private int currentStep = 0;

    private async void Start()
    {
        await UFeelAPI.StartAPI();

        ApplyStep();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.N))
        {
            currentStep++;
            currentStep = Mathf.Clamp(currentStep, 0, 6);

            ApplyStep();
        }

        if (Input.GetKeyUp(KeyCode.B))
        {
            currentStep--;
            currentStep = Mathf.Clamp(currentStep, 0, 6);

            ApplyStep();
        }
    }

    private static void StopUnity()
    {
        UFeelAPI.StopAPI();
        UFeelAPI.Status();

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void StopAllDetections()
    {
        UFeelAPI.StopEmotionDetection();
        UFeelAPI.StopEyeTrackingDetection();
        UFeelAPI.StopSpeechDetection();
        UFeelAPI.StopHeartRateDetection();
    }

    private void ApplyStep()
    {
        StopAllDetections();

        switch (currentStep)
        {
            case 0:
                break;

            case 1:
                Debug.Log("=== Emotion Detection ===");
                UFeelAPI.StartEmotionDetection();
                break;

            case 2:
                Debug.Log("=== Eye Tracking Detection ===");
                UFeelAPI.StartEyeTrackingDetection();
                break;

            case 3:
                Debug.Log("=== Speech Detection ===");
                UFeelAPI.StartSpeechDetection();
                break;

            case 4:
                Debug.Log("=== Heart Rate Detection ===");
                UFeelAPI.StartHeartRateDetection();
                break;

            case 5:
                break;

            case 6:
                Debug.Log("=== End ===");
                StopUnity();
                break;
        }
    }
}
