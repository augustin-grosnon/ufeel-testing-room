namespace UFeel
{
    [System.Serializable]
    public struct EyeTrackingData
    {
        public bool left;
        public bool right;
        // public bool up;
        // public bool down;
        public bool center;

        public bool blink;

        public override readonly string ToString()
        {
            // return $"EyeTrackingData: Left: {left:F2}, Right: {right:F2}, Up: {up:F2}, Down: {down:F2}, Center: {center:F2}";
            return $"EyeTrackingData: Left: {left}, Right: {right}, Center: {center}, Blink: {blink}";
        }

        public enum EyeTrackingDirection
        {
            None,
            Center,
            Left,
            Right,
            // Up,
            // Down,
            // UpLeft,
            // UpRight,
            // DownLeft,
            // DownRight,
        }

        public readonly EyeTrackingDirection CurrentEyeTrackingDirection
        {
            get
            {
                (bool Condition, EyeTrackingDirection Direction)[] cases = new (bool, EyeTrackingDirection)[]
                {
                    (center, EyeTrackingDirection.Center),

                    // Combinaisons
                    // (up && left, EyeTrackingDirection.UpLeft),
                    // (up && right, EyeTrackingDirection.UpRight),
                    // (down && left, EyeTrackingDirection.DownLeft),
                    // (down && right, EyeTrackingDirection.DownRight),

                    // Directions simples
                    // (up, EyeTrackingDirection.Up),
                    // (down, EyeTrackingDirection.Down),
                    (left, EyeTrackingDirection.Left),
                    (right, EyeTrackingDirection.Right),
                };

                foreach ((bool condition, EyeTrackingDirection direction) in cases)
                {
                    if (condition)
                        return direction;
                }

                return EyeTrackingDirection.None;
            }
        }
    }
}
