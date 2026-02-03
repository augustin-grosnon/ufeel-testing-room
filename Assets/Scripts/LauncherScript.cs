using UnityEngine;
using UFeel;
using System.Threading.Tasks;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class LauncherScript : MonoBehaviour
{
    void StopUnity(UFeelAPI instance)
    {
        instance.StopAPI();
        instance.Status();
        Debug.Log("Testing UFEEL Script");
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    async void Start()
    {
        UFeelAPI instance = UFeelAPI.Instance;
        Debug.Log("Hello UFEEL User");
        await Task.Delay(5000);

        instance.StartEmotionDetection();
        instance.Status();

        await Task.Delay(5000);

        Debug.Log("Here is the current emotion " + instance.GetCurrentEmotions());
        Debug.Log("Here is the dominant emotion " + instance.GetDominantEmotion());

        instance.TriggerActionOnEmotionOnce(EmotionData.EmotionType.Anger, async () =>
        {
            instance.StopEmotionDetection();
            instance.Status();
            instance.StartEyeTrackingDetection();
            instance.Status();

            await Task.Delay(5000);

            Debug.Log("Here is the current eye data " + instance.GetCurrentDirections());
            Debug.Log("Here is the dominant direction " + instance.GetDominantDirection());

            instance.TriggerActionOnDirectionOnce(EyeTrackingData.EyeTrackingType.UpRight, () =>
            {
                instance.StopEyeTrackingDetection();

                instance.StartSpeechDetection();

                // TMP
                instance.StartEmotionDetection();
                UFeelAPI.RuleHandle rd = instance.TriggerActionOnEmotionContinuous(EmotionData.EmotionType.Happiness, async () =>
                {
                    await Task.Delay(1000);
                    Debug.Log("Emotion Continuellement");
                });
                //

                instance.Status();

                instance.TriggerActionOnSpeechOnce("Camion", async () =>
                {
                    Debug.Log("Here is the current speech " + instance.GetCurrentSpeech());

                    // TMP
                    instance.RemoveRule(rd);
                    instance.StopEmotionDetection();
                    //

                    instance.StopSpeechDetection();
                    instance.StartHeartRateDetection();
                    instance.Status();

                    await Task.Delay(5000);

                    instance.TriggerActionOnHeartRateOnce(80, () =>
                    {
                        StopUnity(instance);
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
