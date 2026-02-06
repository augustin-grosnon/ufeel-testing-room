using UnityEngine;
using System.Text;

public class HeartRateSensorReceiver : ClientBase
{
    public UFeel.HeartRateSensorData? CurrentHeartRateSensorData { get; private set; } = null;

    public HeartRateSensorReceiver(int port) : base(port)
    {
        PythonServerController.Instance.EnsureServerRunning();
    }

    protected override void ProcessData(byte[] data)
    {
        string json = Encoding.ASCII.GetString(data);
        try
        {
            CurrentHeartRateSensorData = JsonUtility.FromJson<UFeel.HeartRateSensorData>(json);
        }
        catch (System.Exception e)
        {
            Debug.Log("Error parsing speechToText JSON: " + e.Message);
        }
    }
}
