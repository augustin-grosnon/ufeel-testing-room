using System.Collections.Generic;
using System.Linq;

namespace UFeel
{
    [System.Serializable]
    public struct EmotionData
    {
        public float angry;
        public float contemptuous;
        public float disgusted;
        public float fearful;
        public float happy;
        public float neutral;
        public float sad;
        public float surprised;

        public override readonly string ToString()
        {
            return $"Anger: {angry:F2}, " +
                $"Contempt: {contemptuous:F2}, " +
                $"Disgust: {disgusted:F2}, " +
                $"Fear: {fearful:F2}, " +
                $"Happiness: {happy:F2}, " +
                $"Neutral: {neutral:F2}, " +
                $"Sadness: {sad:F2}, " +
                $"Surprise: {surprised:F2}";
        }

        public enum EmotionType
        {
            None,
            Anger,
            Contempt,
            Disgust,
            Fear,
            Happiness,
            Neutral,
            Sadness,
            Surprise,
        }

        public readonly EmotionType GetDominantEmotion()
        {
            Dictionary<EmotionType, float> emotions = new()
            {
                { EmotionType.Anger, angry },
                { EmotionType.Contempt, contemptuous },
                { EmotionType.Disgust, disgusted },
                { EmotionType.Fear, fearful },
                { EmotionType.Happiness, happy },
                { EmotionType.Neutral, neutral },
                { EmotionType.Sadness, sad },
                { EmotionType.Surprise, surprised },
            };

            KeyValuePair<EmotionType, float> maxEntry = emotions.OrderByDescending(kv => kv.Value).First();
            return maxEntry.Value > 0f ? maxEntry.Key : EmotionType.None;
        }
    }
}