using UnityEngine;
using System.Text;

internal class EyeTrackingReceiver : ClientBase
{
    public UFeel.EyeTrackingData? CurrentEyeTrackingData { get; private set; } = null;

    public EyeTrackingReceiver(int port) : base(port)
    {
        PythonServerController.Instance.EnsureServerRunning();
    }

    protected override void ProcessData(byte[] data)
    {
        string json = Encoding.ASCII.GetString(data);
        try
        {
            CurrentEyeTrackingData = JsonUtility.FromJson<UFeel.EyeTrackingData>(json);
        }
        catch (System.Exception e)
        {
            Debug.Log("Error parsing eye direction JSON: " + e.Message);
        }
    }
}
