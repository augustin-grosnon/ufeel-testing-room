using UnityEngine;
using System.Text;

internal class EmotionReceiver : ClientBase
{
    public UFeel.EmotionData? CurrentEmotionsData { get; private set; } = null;

    public EmotionReceiver(int port) : base(port)
    {
        PythonServerController.Instance.EnsureServerRunning();
    }

    protected override void ProcessData(byte[] data)
    {
        string json = Encoding.ASCII.GetString(data);
        try
        {
            CurrentEmotionsData = JsonUtility.FromJson<UFeel.EmotionData>(json);
        }
        catch (System.Exception e)
        {
            Debug.Log("Error parsing emotion JSON: " + e.Message);
        }
    }
}
