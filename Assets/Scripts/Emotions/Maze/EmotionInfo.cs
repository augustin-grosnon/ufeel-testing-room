using UFeel;
using UnityEngine;

public readonly struct EmotionInfo
{
    public EmotionData.EmotionType Emotion { get; }
    public Color Color { get; }

    public EmotionInfo(EmotionData.EmotionType emotion, Color color)
    {
        Emotion = emotion;
        Color = color;
    }

    public static EmotionInfo GetRandomEmotion()
    {
        return Emotions[Random.Range(0, Emotions.Length)];
    }

    public static readonly EmotionInfo[] Emotions =
    {
        new(
            EmotionData.EmotionType.Happiness,
            new Color(1f, 1f, 0f, 0.4f)
        ),
        new(
            EmotionData.EmotionType.Surprise,
            new Color(1f, 0.5f, 0f, 0.4f)
        ),
        new(
            EmotionData.EmotionType.Fear,
            new Color(0f, 0f, 1f, 0.4f)
        )
    };
}
