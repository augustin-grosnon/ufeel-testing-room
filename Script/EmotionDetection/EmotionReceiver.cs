using UnityEngine;
using System.Text;

[System.Serializable]
public class EmotionData
{
    public float happiness { get; set; }
    public float surprise { get; set; }
    public float sadness { get; set; }
    public float anger { get; set; }
    public float neutral { get; set; }
    public float fear { get; set; }

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

        var max = emotions.MaxBy(kv => kv.Value);
        return max.Value > 0f ? max.Key : EmotionType.None;
    }
}
public class EmotionReceiver : UDPReceiverBase
{
    public EmotionData? _currentEmotions { get; private set; } = null;

    public EmotionReceiver(int port) : base(port)
    {
        PythonServerController.Instance.EnsureServerRunning();
    }

    protected override void ProcessData(byte[] data)
    {
        Debug.Log("Here is data" + data);
        // check every time we need to process the data if the server is running ?
        // _emotionServerPython.EnsureServerRunning();
        string json = Encoding.ASCII.GetString(data);
        try
        {
            _currentEmotions = JsonUtility.FromJson<EmotionData>(json);
        }
        catch (System.Exception e)
        {
            Debug.Log("Error parsing emotion JSON: " + e.Message);
        }
    }
}
