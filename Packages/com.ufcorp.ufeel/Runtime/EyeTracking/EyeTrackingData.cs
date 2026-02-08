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

        public override readonly string ToString()
        {
            return $"EyeTrackingData: Left: {left}, Right: {right}, Up: {up}, Down: {down}, Center: {center}";
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

        public readonly EyeTrackingType GetEyeTrackingType()
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