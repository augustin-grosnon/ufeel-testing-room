using UnityEngine;
using UFeel;
using System.Threading.Tasks;
using System.Collections;

#if UNITY_EDITOR
    using UnityEditor;
#endif

public class LauncherScript : MonoBehaviour
{
    IEnumerator WaitUntilNotAnger(UfeelAPI instance, System.Action onFinished)
    {
        bool looping = true;

        Debug.Log("Waiting until emotion is Anger!");
        while (looping)
        {
            instance.TriggerActionIfEmotion(EmotionData.EmotionType.Anger, () =>
            {
                looping = false;
            });
            yield return new WaitForSeconds(0.5f);
        }
        Debug.Log("Emotion is Anger!");
        onFinished?.Invoke();
    }

    IEnumerator WaitUntilUpRight(UfeelAPI instance, System.Action onFinished)
    {
        bool looping = true;

        Debug.Log("Waiting until eye is Up Right!");
        while (looping)
        {
            instance.TriggerActionIfDirection(EyeTrackingData.EyeTrackingType.UpRight, () =>
            {
                looping = false;
            });
            yield return new WaitForSeconds(0.5f);
        }
        Debug.Log("Eye is Up Right!");
        onFinished?.Invoke();
    }

    IEnumerator WaitUntilWords(UfeelAPI instance, System.Action onFinished)
    {
        bool looping = true;

        Debug.Log("Waiting until word is Camion!");
        while (looping)
        {
            instance.TriggerActionIfSpeech("Camion", () =>
            {
                looping = false;
            });
            yield return new WaitForSeconds(0.5f);
        }
        Debug.Log("He said the word!");
        onFinished?.Invoke();
    }

    IEnumerator WaitUntilHeartRate(UfeelAPI instance, System.Action onFinished)
    {
        bool looping = true;

        Debug.Log("Waiting until heart rate is 80!");
        while (looping)
        {
            instance.TriggerActionIfHeartRate(80, () =>
            {
                looping = false;
            }, 5);
            yield return new WaitForSeconds(0.5f);
        }
        Debug.Log("He is really calm (80 heart rate)!");
        onFinished?.Invoke();
    }

    void StopUnity(UfeelAPI instance)
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
        UfeelAPI instance = UfeelAPI.Instance;
        Debug.Log("Hello UFEEL User");
        await Task.Delay(5000);

        instance.StartEmotionDetection();
        instance.Status();

        await Task.Delay(5000);

        Debug.Log("Here is the current emotion " + instance.GetCurrentEmotions());
        Debug.Log("Here is the dominant emotion " + instance.GetDominantEmotion());

        StartCoroutine(WaitUntilNotAnger(instance, async () =>
        {
            instance.StopEmotionDetection();
            instance.Status();
            instance.StartEyeTrackingDetection();
            instance.Status();

            await Task.Delay(5000);

            Debug.Log("Here is the current eye data " + instance.GetCurrentDirections());
            Debug.Log("Here is the dominant direction " + instance.GetDominantDirection());

            StartCoroutine(WaitUntilUpRight(instance, () =>
            {
                instance.StopEyeTrackingDetection();

                instance.StartSpeechDetection();
                instance.Status();

                StartCoroutine(WaitUntilWords(instance, async () =>
                {
                    Debug.Log("Here is the current speech " + instance.GetCurrentSpeech());
                    instance.StopSpeechDetection();
                    instance.StartHeartRateDetection();
                    instance.Status();

                    await Task.Delay(5000);

                    StartCoroutine(WaitUntilHeartRate(instance, () =>
                    {
                        StopUnity(instance);
                    }));
                }));
            }));
        }));
    }

    void Update()
    {
        return;
    }
}
