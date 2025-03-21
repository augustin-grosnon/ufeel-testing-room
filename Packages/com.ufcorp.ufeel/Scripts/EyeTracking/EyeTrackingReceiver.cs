using UnityEngine;
using System.Text;

[System.Serializable]
public class EyeDirectionData
{
    public bool left;
    public bool right;
    public bool up;
    public bool down;
    public bool center;
}

public class EyeTrackingReceiver : UDPReceiverBase
{
    private static EyeTrackingReceiver _instance;

    public static EyeTrackingReceiver Instance
    {
        get
        {
            _instance ??= new EyeTrackingReceiver();
            return _instance;
        }
    }

    public static EyeDirectionData CurrentEyeData { get; private set; } = new();

    private EyeTrackingReceiver() : base(4242)
    {
        PythonServerController.Instance.EnsureServerRunning();
    }

    protected override void ProcessData(byte[] data)
    {
        string json = Encoding.ASCII.GetString(data);
        try
        {
            CurrentEyeData = JsonUtility.FromJson<EyeDirectionData>(json);
        }
        catch (System.Exception e)
        {
            Debug.Log("Error parsing eye direction JSON: " + e.Message);
        }
    }
}
