using UnityEngine;
class Program
{
    static void Main(string[] args)
    {
        UfeelAPI instance = UfeelAPI.Instance;

        instance.StartEmotionDetection();
        instance.status();
        instance.StartEyeTrackingDetection();

        // need to sleep to be sure the python server is start for the two print
        Thread.Sleep(5_000);

        Debug.Log("Here is the current emotion " + instance.GetCurrentEmotions());
        Debug.Log("Here is the dominant emotion " + instance.GetDominantEmotion());

        bool looping = true;
        while (looping)
        {
            instance.TriggerActionIfEmotion(EmotionData.EmotionType.Anger, () =>
            {
                looping = false;
            });
        }

        instance.StopEmotionDetection();
        instance.status();
        instance.StopEmotionDetection();

        Debug.Log("Here is the current eye data " + instance.GetCurrentDirections());
        Debug.Log("Here is the dominant emotion " + instance.GetDominantDirection());

        instance.StartEmotionDetection();
        instance.stopAPI();
        instance.status();

        Debug.Log("Testing UFEEL Script");

        // Thread.Sleep(5_000);
    }
}