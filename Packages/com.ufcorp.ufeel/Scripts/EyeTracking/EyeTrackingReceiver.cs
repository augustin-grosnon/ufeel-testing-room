using UnityEngine;
using System.Text;

[System.Serializable]
public class EyeTrackingData
{
    public bool Left { get; set; }
    public bool Right { get; set; }
    public bool Up { get; set; }
    public bool Down { get; set; }
    public bool Center { get; set; }

    public override string ToString()
    {
        return $"EyeTrackingData: " +
            $"Left: {Left}, Right: {Right}, Up: {Up}, Down: {Down}, Center: {Center}";
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
            (Center, EyeTrackingType.Center),

            // Combinaisons
            (Up && Left, EyeTrackingType.UpLeft),
            (Up && Right, EyeTrackingType.UpRight),
            (Down && Left, EyeTrackingType.DownLeft),
            (Down && Right, EyeTrackingType.DownRight),

            // Directions simples
            (Up, EyeTrackingType.Up),
            (Down, EyeTrackingType.Down),
            (Left, EyeTrackingType.Left),
            (Right, EyeTrackingType.Right),
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
    public EyeTrackingData CurrentEyeTrackingData { get; private set; } = new();

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
