using UnityEngine;
using System.Text;

namespace UFeel
{
    [System.Serializable]
    public struct HeartRateData
    {
        public int rate;
    }
}

public class HeartRateSensorReceiver : ClientBase
{
    public UFeel.HeartRateData? CurrentHeartRateData { get; private set; } = null;

    public HeartRateSensorReceiver(int port) : base(port)
    {
        PythonServerController.Instance.EnsureServerRunning();
    }

    protected override void ProcessData(byte[] data)
    {
        string json = Encoding.ASCII.GetString(data);
        try
        {
            CurrentHeartRateData = JsonUtility.FromJson<UFeel.HeartRateData>(json);
        }
        catch (System.Exception e)
        {
            Debug.Log("Error parsing speechToText JSON: " + e.Message);
        }
    }
}
