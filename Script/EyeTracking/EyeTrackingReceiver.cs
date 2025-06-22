using UnityEngine;
using System.Text;

[System.Serializable]
public class EyeTrackingData
{
    public bool left { get; set; }
    public bool right { get; set; }
    public bool up { get; set; }
    public bool down { get; set; }
    public bool center { get; set; }

    public override string ToString()
    {
        return $"EyeTrackingData: " +
            $"Left: {left}, Right: {right}, Up: {up}, Down: {down}, Center: {center}";
    }

    public enum EyeTrackingType
    {
        None,
        Center,
        Left,
        Right,
        Up,
        Down,
        UpLeft,
        UpRight,
        DownLeft,
        DownRight,
    }

    public EyeTrackingType GetEyeTrackingType()
    {
        (bool condition, EyeTrackingType type)[] cases = new (bool, EyeTrackingType)[]
        {
            (center, EyeTrackingType.Center),

            // Combinaisons
            (up && left, EyeTrackingType.UpLeft),
            (up && right, EyeTrackingType.UpRight),
            (down && left, EyeTrackingType.DownLeft),
            (down && right, EyeTrackingType.DownRight),

            // Directions simples
            (up, EyeTrackingType.Up),
            (down, EyeTrackingType.Down),
            (left, EyeTrackingType.Left),
            (right, EyeTrackingType.Right),
        };

        foreach (var (condition, type) in cases)
        {
            if (condition)
                return type;
        }

        return EyeTrackingType.None;
    }
}

public class EyeTrackingReceiver : ClientBase
{
    public EyeTrackingData? CurrentEyeTrackingData { get; private set; } = new();

    public EyeTrackingReceiver(int port) : base(port)
    {
        PythonServerController.Instance.EnsureServerRunning();
    }

    protected override void ProcessData(byte[] data)
    {
        string json = Encoding.ASCII.GetString(data);
        try
        {
            CurrentEyeTrackingData = JsonUtility.FromJson<EyeTrackingData>(json);
        }
        catch (System.Exception e)
        {
            Debug.Log("Error parsing eye direction JSON: " + e.Message);
        }
    }
}
