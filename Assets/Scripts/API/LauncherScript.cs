using UnityEngine;
using UFeel;
using System.Threading.Tasks;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class LauncherScript : MonoBehaviour
{
    void StopUnity()
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

    async void Start()
    {
        await UFeelAPI.StartAPI();

        UFeelAPI.StartEmotionDetection();
        UFeelAPI.Status();

        Debug.Log("Here is the current emotion " + UFeelAPI.GetCurrentEmotionsData());
        Debug.Log("Here is the dominant emotion " + UFeelAPI.GetDominantEmotion());

        UFeelAPI.TriggerActionOnEmotionOnce(EmotionData.EmotionType.Anger, () =>
        {
            UFeelAPI.StopEmotionDetection();
            UFeelAPI.Status();
            UFeelAPI.StartEyeTrackingDetection();
            UFeelAPI.Status();

            Debug.Log("Here is the current eye data " + UFeelAPI.GetCurrentDirections());
            Debug.Log("Here is the dominant direction " + UFeelAPI.GetDominantDirection());

            UFeelAPI.TriggerActionOnDirectionOnce(EyeTrackingData.EyeTrackingType.UpRight, () =>
            {
                UFeelAPI.StopEyeTrackingDetection();

                UFeelAPI.StartSpeechDetection();

                // Continuous Emotion
                UFeelAPI.StartEmotionDetection();
                RuleKey key = UFeelAPI.TriggerActionOnEmotionContinuous(EmotionData.EmotionType.Happiness, async () =>
                {
                    await Task.Delay(1000);
                    Debug.Log("Emotion Continuellement");
                });
                //

                UFeelAPI.Status();

                UFeelAPI.TriggerActionOnSpeechOnce("Camion", () =>
                {
                    Debug.Log("Here is the current speech " + UFeelAPI.GetCurrentSpeech());

                    // Remove Continuous Emotion
                    UFeelAPI.RemoveRule(key);
                    UFeelAPI.StopEmotionDetection();
                    //

                    UFeelAPI.StopSpeechDetection();
                    UFeelAPI.StartHeartRateDetection();
                    UFeelAPI.Status();

                    UFeelAPI.TriggerActionOnHeartRateOnce(80, () =>
                    {
                        StopUnity();
                    });
                });
            });
        });
    }

    void Update()
    {
        return;
    }
}
