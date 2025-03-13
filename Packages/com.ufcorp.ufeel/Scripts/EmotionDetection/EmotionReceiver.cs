using UnityEngine;
using System.Text;

[System.Serializable]
public class EmotionData
{
    public float happiness;
    public float surprise;
    public float sadness;
    public float anger;
    public float disgust;
    public float fear;
}

public class EmotionReceiver : UDPReceiverBase
{
    public EmotionServerController emotionController = new();
    public EmotionData emotionData = new();

    protected override void Setup()
    {
        if (emotionController != null)
        {
            emotionController.EnsureServerRunning();
        }
        else
        {
            Debug.LogWarning("EmotionServerController reference not set in EmotionReceiver.");
        }

        port = 4243;
        base.Setup();
    }

    protected override void ProcessData(byte[] data)
    {
        string json = Encoding.ASCII.GetString(data);
        try
        {
            emotionData = JsonUtility.FromJson<EmotionData>(json);
        }
        catch (System.Exception e)
        {
            Debug.Log("Error parsing emotion JSON: " + e.Message);
        }
    }
}
