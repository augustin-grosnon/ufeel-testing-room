using UnityEngine;
using System.Text;

[System.Serializable]
public struct SpeechData
{
    public string text;
}

public class SpeechToTextReceiver : ClientBase
{
    public SpeechData? CurrentSpeechData { get; private set; } = null;

    public SpeechToTextReceiver(int port) : base(port)
    {
        Debug.Log("[SpeechToTextReceiver] Constructor called — listening on port " + port);
        PythonServerController.Instance.EnsureServerRunning();
    }

    protected override void ProcessData(byte[] data)
    {
        string json = Encoding.ASCII.GetString(data);
        try
        {
            CurrentSpeechData = JsonUtility.FromJson<SpeechData>(json);
        }
        catch (System.Exception e)
        {
            Debug.Log("Error parsing speechToText JSON: " + e.Message);
        }
    }
}
