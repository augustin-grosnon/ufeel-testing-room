using System.Threading.Tasks;
using UFeel;
using UnityEditor;
using UnityEngine;

public class LauncherScript : MonoBehaviour
{
    private int currentStep = -1;

    private async void Start()
    {
        await UFeelAPI.StartAPI().ConfigureAwait(true);

        await Task.Delay(millisecondsDelay: 5000).ConfigureAwait(true);

        NextStep();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.N))
        {
            NextStep();
        }
    }

    private static void StopUnity()
    {
        UFeelAPI.StopAPI();
        UFeelAPI.Status();
        Debug.Log("Testing UFEEL Script");
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void NextStep()
    {
        currentStep++;

        switch (currentStep)
        {
            case 0:
                Debug.Log("=== Emotion Detection ===");

                UFeelAPI.StartEmotionDetection();
                UFeelAPI.Status();

                Debug.Log("Current emotion: " + UFeelAPI.CurrentEmotionsData);
                Debug.Log("Dominant emotion: " + UFeelAPI.DominantEmotion);
                break;

            case 1:
                Debug.Log("=== Eye Tracking Detection ===");

                UFeelAPI.StopEmotionDetection();

                UFeelAPI.StartEyeTrackingDetection();
                UFeelAPI.Status();

                Debug.Log("Current eye data: " + UFeelAPI.CurrentDirections);
                Debug.Log("Dominant direction: " + UFeelAPI.DominantDirection);
                break;

            case 2:
                Debug.Log("=== Speech Detection ===");

                UFeelAPI.StopEyeTrackingDetection();

                UFeelAPI.StartSpeechDetection();
                UFeelAPI.Status();
                break;

            case 3:
                Debug.Log("Current speech: " + UFeelAPI.CurrentSpeech);

                UFeelAPI.StopSpeechDetection();

                UFeelAPI.StartHeartRateDetection();
                UFeelAPI.Status();
                break;

            case 4:
                Debug.Log("=== Heart Rate Detection ===");

                Debug.Log("Current heart rate: " + UFeelAPI.CurrentHeartRate);
                break;

            case 5:
                Debug.Log("=== End ===");

                UFeelAPI.StopHeartRateDetection();
                StopUnity();
                break;
        }
    }
}