using UnityEngine;
using UFeel;
using System.Threading.Tasks;
using System.Collections;

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

    async void Start()
    {
        UfeelAPI instance = UfeelAPI.Instance;
        Debug.Log("Hello UFEEL User");
        await Task.Delay(5000);

        // instance.StartEmotionDetection();
        // instance.Status();

        // await Task.Delay(5000);

        // Debug.Log("Here is the current emotion " + instance.GetCurrentEmotions());
        // Debug.Log("Here is the dominant emotion " + instance.GetDominantEmotion());

        // StartCoroutine(WaitUntilNotAnger(instance, async () =>
        // {
        //     instance.StopEmotionDetection();
        //     instance.Status();
            instance.StartEyeTrackingDetection();
            instance.Status();

            await Task.Delay(5000);

            Debug.Log("Here is the current eye data " + instance.GetCurrentDirections());
            Debug.Log("Here is the dominant emotion " + instance.GetDominantDirection());

            StartCoroutine(WaitUntilUpRight(instance, () =>
            {
                instance.StopEyeTrackingDetection();
                instance.StopAPI();
                instance.Status();
                Debug.Log("Testing UFEEL Script");
            }));
        // }));
    }

    void Update()
    {
        return;
    }
}
