using UnityEngine;
using System.Text;

internal class SpeechToTextReceiver : ClientBase
{
    public UFeel.SpeechToTextData? CurrentSpeechData { get; private set; } = null;

    public SpeechToTextReceiver(int port) : base(port)
    {
        PythonServerController.Instance.EnsureServerRunning();
    }

    protected override void ProcessData(byte[] data)
    {
        string json = Encoding.ASCII.GetString(data);
        try
        {
            CurrentSpeechData = JsonUtility.FromJson<UFeel.SpeechToTextData>(json);
        }
        catch (System.Exception e)
        {
            Debug.Log("Error parsing speechToText JSON: " + e.Message);
        }
    }
}
