using UnityEngine;
using System.Text;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class EmotionData
{
    public float Happiness { get; set; }
    public float Surprise { get; set; }
    public float Sadness { get; set; }
    public float Anger { get; set; }
    public float Neutral { get; set; }
    public float Fear { get; set; }

    public override string ToString()
    {
        return $"Happiness: {Happiness}, Surprise: {Surprise}, Sadness: {Sadness}, " +
            $"Anger: {Anger}, Neutral: {Neutral}, Fear: {Fear}";
    }

    public enum EmotionType
    {
        None,
        Happiness,
        Surprise,
        Sadness,
        Anger,
        Neutral,
        Fear
    }

    public EmotionType GetDominantEmotion()
    {
        Dictionary<EmotionType, float> emotions = new()
        {
            { EmotionType.Happiness, Happiness },
            { EmotionType.Surprise, Surprise },
            { EmotionType.Sadness, Sadness },
            { EmotionType.Anger, Anger },
            { EmotionType.Neutral, Neutral },
            { EmotionType.Fear, Fear }
        };


        var maxEntry = emotions.OrderByDescending(kv => kv.Value).First();
        return maxEntry.Value > 0f ? maxEntry.Key : EmotionType.None;
    }
}
public class EmotionReceiver : ClientBase
{
    public EmotionData CurrentEmotions { get; private set; } = null;

    public EmotionReceiver(int port) : base(port)
    {
        PythonServerController.Instance.EnsureServerRunning();
    }

    protected override void ProcessData(byte[] data)
    {
        // check every time we need to process the data if the server is running ?
        // _emotionServerPython.EnsureServerRunning();
        string json = Encoding.ASCII.GetString(data);
        try
        {
            CurrentEmotions = JsonUtility.FromJson<EmotionData>(json);
        }
        catch (System.Exception e)
        {
            Debug.Log("Error parsing emotion JSON: " + e.Message);
        }
    }
}
