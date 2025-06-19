using UnityEngine;

public class EmotionManager : MonoBehaviour
{
    [SerializeField] private CubeColorChanger _cubeColorChanger;

    void Awake()
    {
        var _ = EmotionReceiver.Instance;
    }

    void LateUpdate()
    {
        if (_cubeColorChanger == null)
            return;

        string dominantEmotion = GetDominantEmotion(EmotionReceiver.CurrentEmotions);
        _cubeColorChanger.SetColor(dominantEmotion);
    }

    private string GetDominantEmotion(EmotionData data)
    {
        float[] percentages = {
            data.happiness,
            data.surprise,
            data.sadness,
            data.anger,
            data.neutral,
            data.fear,
            data.disgust
        };
        string[] labels = { "Happiness", "Surprise", "Sadness", "Anger", "Neutral", "Fear", "Disgust" };

        int maxIndex = 0;
        for (int i = 1; i < percentages.Length; i++)
        {
            if (percentages[i] > percentages[maxIndex])
                maxIndex = i;
        }
        return labels[maxIndex];
    }
}
