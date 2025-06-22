using UnityEngine;
class Program
{
    static void Main(string[] args)
    {
        UfeelAPI instance = UfeelAPI.Instance;

        Thread.Sleep(5_000);

        instance.StartEmotionDetection();
        instance.Status();

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
        instance.Status();
        instance.StopEmotionDetection();

        instance.StartEyeTrackingDetection();
        Thread.Sleep(5_000);

        Debug.Log("Here is the current eye data " + instance.GetCurrentDirections());
        Debug.Log("Here is the dominant emotion " + instance.GetDominantDirection());

        looping = true;
        while (looping)
        {
            instance.TriggerActionIfDirection(EyeTrackingData.EyeTrackingType.UpRight, () =>
            {
                looping = false;
            });
        }


        instance.StartEmotionDetection();
        instance.StopAPI();
        instance.Status();

        Debug.Log("Testing UFEEL Script");

        // Thread.Sleep(5_000);
    }
}