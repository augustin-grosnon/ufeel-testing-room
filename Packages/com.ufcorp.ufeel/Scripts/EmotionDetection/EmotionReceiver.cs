using UnityEngine;
using System.Text;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public struct EmotionData
{
    public float happiness;
    public float surprise;
    public float sadness;
    public float anger;
    public float neutral;
    public float fear;

    public override string ToString()
    {
        return $"Happiness: {happiness}, Surprise: {surprise}, Sadness: {sadness}, " +
            $"Anger: {anger}, Neutral: {neutral}, Fear: {fear}";
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
            { EmotionType.Happiness, happiness },
            { EmotionType.Surprise, surprise },
            { EmotionType.Sadness, sadness },
            { EmotionType.Anger, anger },
            { EmotionType.Neutral, neutral },
            { EmotionType.Fear, fear }
        };


        var maxEntry = emotions.OrderByDescending(kv => kv.Value).First();
        return maxEntry.Value > 0f ? maxEntry.Key : EmotionType.None;
    }
}
public class EmotionReceiver : ClientBase
{
    public EmotionData? CurrentEmotions { get; private set; } = null;

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
