using UnityEngine;
using UFeel;
using System.Threading.Tasks;

public class LauncherScript : MonoBehaviour
{
    async void Start()
    {
        UfeelAPI instance = UfeelAPI.Instance;

        Debug.Log("Hello UFEEL User");
        // await Task.Delay(100);

        instance.StartEmotionDetection();
        instance.Status();

        await Task.Delay(5000);

        Debug.Log("Here is the current emotion " + instance.GetCurrentEmotions());
        Debug.Log("Here is the dominant emotion " + instance.GetDominantEmotion());

        // bool looping = true;
        // while (looping)
        // {
        //     instance.TriggerActionIfEmotion(EmotionData.EmotionType.Anger, () =>
        //     {
        //         looping = false;
        //     });
        // }


        // instance.StopEmotionDetection();
        // instance.Status();
        // instance.StopEmotionDetection();

        // instance.StartEyeTrackingDetection();
        // await Task.Delay(5000);

        // Debug.Log("Here is the current eye data " + instance.GetCurrentDirections());
        // Debug.Log("Here is the dominant emotion " + instance.GetDominantDirection());

        // looping = true;
        // while (looping)
        // {
        //     instance.TriggerActionIfDirection(EyeTrackingData.EyeTrackingType.UpRight, () =>
        //     {
        //         looping = false;
        //     });
        // }


        // instance.StartEmotionDetection();
        // instance.StopAPI();
        // instance.Status();

        Debug.Log("Testing UFEEL Script");

    }

    void Update()
    {
        return;
    }
}
