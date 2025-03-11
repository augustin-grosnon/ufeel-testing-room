using UnityEngine;

public class EmotionManager : MonoBehaviour
{
    [SerializeField] private EmotionReceiver emotionReceiver;
    [SerializeField] private CubeColorChanger cubeColorChanger;

    void LateUpdate()
    {
        if (emotionReceiver == null || cubeColorChanger == null)
            return;

        string dominantEmotion = GetDominantEmotion(emotionReceiver.emotionData);
        cubeColorChanger.SetColor(dominantEmotion);
    }

    private string GetDominantEmotion(EmotionData data)
    {
        float[] percentages = {
            data.happiness,
            data.surprise,
            data.sadness,
            data.anger,
            data.disgust,
            data.fear
        };
        string[] labels = { "Happiness", "Surprise", "Sadness", "Anger", "Disgust", "Fear" };

        int maxIndex = 0;
        for (int i = 1; i < percentages.Length; i++)
        {
            if (percentages[i] > percentages[maxIndex])
                maxIndex = i;
        }
        return labels[maxIndex];
    }
}
