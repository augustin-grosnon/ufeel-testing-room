using UnityEngine;
using System.Text;

namespace UFeel
{
    [System.Serializable]
    public struct SpeechData
    {
        public string text;
    }
}

internal class SpeechToTextReceiver : ClientBase
{
    public UFeel.SpeechData? CurrentSpeechData { get; private set; } = null;

    public SpeechToTextReceiver(int port) : base(port)
    {
        PythonServerController.Instance.EnsureServerRunning();
    }

    protected override void ProcessData(byte[] data)
    {
        string json = Encoding.ASCII.GetString(data);
        try
        {
            CurrentSpeechData = JsonUtility.FromJson<UFeel.SpeechData>(json);
        }
        catch (System.Exception e)
        {
            Debug.Log("Error parsing speechToText JSON: " + e.Message);
        }
    }
}
