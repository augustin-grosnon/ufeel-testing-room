using System;
using System.Collections.Generic;
using System.Linq;

namespace UFeel
{
    [Serializable]
    public struct EmotionData
    {
        [Serializable]
        public struct Emotion
        {
            public float value;
            public bool enabled;
        }

        public Emotion angry;
        public Emotion contemptuous;
        public Emotion disgusted;
        public Emotion fearful;
        public Emotion happy;
        public Emotion neutral;
        public Emotion sad;
        public Emotion surprised;

        public override readonly string ToString()
        {
            return $"Anger: {angry.value:F2}, " +
                $"Contempt: {contemptuous.value:F2}, " +
                $"Disgust: {disgusted.value:F2}, " +
                $"Fear: {fearful.value:F2}, " +
                $"Happiness: {happy.value:F2}, " +
                $"Neutral: {neutral.value:F2}, " +
                $"Sadness: {sad.value:F2}, " +
                $"Surprise: {surprised.value:F2}";
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

        public readonly EmotionType DominantEmotion
        {
            get
            {
                Dictionary<EmotionType, Emotion> emotions = new()
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

                IEnumerable<KeyValuePair<EmotionType, Emotion>> enabledEmotions = emotions
                    .Where(kv => kv.Value.enabled);

                IEnumerable<KeyValuePair<EmotionType, Emotion>> selected = enabledEmotions.Any()
                    ? enabledEmotions
                    : emotions;

                KeyValuePair<EmotionType, Emotion> maxEntry = selected
                    .OrderByDescending(kv => kv.Value.value)
                    .First();

                return maxEntry.Value.value > 0f
                    ? maxEntry.Key
                    : EmotionType.None;
            }
        }
        // TODO: add options for using or not enabled emotions
    }
}
