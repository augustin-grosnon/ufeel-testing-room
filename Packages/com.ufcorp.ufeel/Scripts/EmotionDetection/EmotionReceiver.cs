using UnityEngine;
using System.Text;

[System.Serializable]
public class EmotionData
{
    public float happiness;
    public float surprise;
    public float sadness;
    public float anger;
    public float neutral;
    public float fear;
}

public class EmotionReceiver : UDPReceiverBase
{
    private static EmotionReceiver _instance;

    public static EmotionReceiver Instance
    {
        get
        {
            _instance ??= new EmotionReceiver();
            return _instance;
        }
    }

    public static EmotionData CurrentEmotions { get; private set; } = new();

    private EmotionReceiver() : base(4243)
    {
        PythonServerController.Instance.EnsureServerRunning();
    }

    protected override void ProcessData(byte[] data)
    {
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
