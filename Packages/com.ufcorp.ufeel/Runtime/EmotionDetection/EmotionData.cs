using System.Collections.Generic;
using System.Linq;

namespace UFeel
{
    [System.Serializable]
    public struct EmotionData
    {
        public float happiness;
        public float surprise;
        public float sadness;
        public float anger;
        public float neutral;
        public float fear;

        public override readonly string ToString()
        {
            return $"Happiness: {happiness}, Surprise: {surprise}, Sadness: {sadness}, " +
                $"Anger: {anger}, Neutral: {neutral}, Fear: {fear}";
        }

        public enum EmotionType
        {
            None,
            Happiness,
            Surprise,
            Sadness,
            Anger,
            Neutral,
            Fear
        }

        public readonly EmotionType GetDominantEmotion()
        {
            Dictionary<EmotionType, float> emotions = new()
            {
                { EmotionType.Happiness, happiness },
                { EmotionType.Surprise, surprise },
                { EmotionType.Sadness, sadness },
                { EmotionType.Anger, anger },
                { EmotionType.Neutral, neutral },
                { EmotionType.Fear, fear }
            };

            var maxEntry = emotions.OrderByDescending(kv => kv.Value).First();
            return maxEntry.Value > 0f ? maxEntry.Key : EmotionType.None;
        }
    }
}