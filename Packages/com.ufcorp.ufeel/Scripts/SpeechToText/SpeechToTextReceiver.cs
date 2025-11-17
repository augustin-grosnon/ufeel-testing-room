using UnityEngine;
using System.Text;
// using System.Diagnostics;

[System.Serializable]
public class TextData
{
    public string text;
}

public class SpeechToTextReceiver : UDPReceiverBase
{
    private static SpeechToTextReceiver _instance;

    public static SpeechToTextReceiver Instance
    {
        get
        {
            _instance ??= new SpeechToTextReceiver();
            return _instance;
        }
    }

    public static TextData CurrentText { get; private set; } = new();

    private SpeechToTextReceiver() : base(4244)
    {
        Debug.Log("[SpeechToTextReceiver] Constructor called — listening on port 4244");   
        PythonServerController.Instance.EnsureServerRunning();
    }

    protected override void ProcessData(byte[] data)
    {
        string json = Encoding.ASCII.GetString(data);

        try
        {
            CurrentText = JsonUtility.FromJson<TextData>(json);
        }
        catch (System.Exception e)
        {
            Debug.Log("Error parsing speechToText JSON: " + e.Message);
            Debug.LogError("Error parsing speechToText JSON: " + e.Message); //
        }
    }
}

