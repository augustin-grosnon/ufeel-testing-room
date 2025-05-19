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

[System.Serializable]
public class EyeDirectionRatio
{
    public float horizontal;
    public float vertical;
}

public enum EyeTrackingError
{
    NONE = 0,
    NO_EYES_DETECTED = 1
}

[System.Serializable]
public class EyeDirectionError
{
    public EyeTrackingError error;
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
    public static EyeDirectionRatio CurrentEyeRatios { get; private set; } = new();
    public static EyeDirectionError Error { get; private set; } = new EyeDirectionError {error = EyeTrackingError.NONE};

    private EyeTrackingReceiver() : base(4242)
    {
        PythonServerController.Instance.EnsureServerRunning();
    }

    public static bool IsError()
    {
        return Error.error != EyeTrackingError.NONE;
    }
    
    protected override void ProcessData(byte[] data)
    {
        string json = Encoding.ASCII.GetString(data);
        try
        {
            CurrentEyeData = JsonUtility.FromJson<EyeDirectionData>(json);
            CurrentEyeRatios = JsonUtility.FromJson<EyeDirectionRatio>(json);
            Error = JsonUtility.FromJson<EyeDirectionError>(json);
        }
        catch (System.Exception e)
        {
            Debug.Log("Error parsing eye direction JSON: " + e.Message);
        }
    }
}
