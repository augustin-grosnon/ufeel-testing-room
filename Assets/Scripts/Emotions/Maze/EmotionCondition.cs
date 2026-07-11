using UFeel;

public class EmotionCondition : FogCondition
{
    private EmotionData.EmotionType _emotionType;

    public EmotionCondition(EmotionData.EmotionType emotionType)
    {
        _emotionType = emotionType;
    }

    public override bool CanPass()
    {
        return LabyrinthManager.GetEmotionLevel(_emotionType) >= 2.0f;
    }
}
