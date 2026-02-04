using UnityEngine;
using System.Text;

namespace UFeel
{
    [System.Serializable]
    public struct EyeTrackingData
    {
        public bool left;
        public bool right;
        public bool up;
        public bool down;
        public bool center;

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
}
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
