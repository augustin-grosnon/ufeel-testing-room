using UnityEngine;
using System.Text;

public class EyeTrackingReceiver : UDPReceiverBase
{
    public EyeTrackingServerController eyeTrackingController;
    public Vector2 gazePosition = new(0.5f, 0.5f);

    protected override void Setup()
    {
        if (eyeTrackingController != null)
        {
            eyeTrackingController.EnsureServerRunning();
        }
        else
        {
            Debug.LogWarning("EyeTrackingServerController reference not set in EyeTrackingReceiver.");
        }

        port = 4242;
        base.Setup();
    }

    protected override void ProcessData(byte[] data)
    {
        string receivedText = Encoding.ASCII.GetString(data);
        string[] parts = receivedText.Split(',');
        if (parts.Length == 2)
        {
            if (float.TryParse(parts[0], out float x) && float.TryParse(parts[1], out float y))
            {
                gazePosition = new Vector2(x, y);
            }
        }
    }
}
