using UnityEngine;
using System.Text;

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

    public static Vector2 CurrentGaze { get; private set; } = new(0.5f, 0.5f);

    private readonly EyeTrackingServerController eyeTrackingController = new();

    private EyeTrackingReceiver() : base(4242)
    {
        if (eyeTrackingController != null)
        {
            eyeTrackingController.EnsureServerRunning();
        }
        else
        {
            Debug.LogWarning("EyeTrackingServerController reference not set in EyeTrackingReceiver.");
        }
    }

    protected override void ProcessData(byte[] data)
    {
        string receivedText = Encoding.ASCII.GetString(data);
        string[] parts = receivedText.Split(',');
        if (parts.Length == 2)
        {
            if (float.TryParse(parts[0], out float x) && float.TryParse(parts[1], out float y))
            {
                CurrentGaze = new Vector2(x, y);
            }
        }
    }
}
