using TMPro;
using UFeel;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [Header("Room Data")]
    public RoomData Data;

    [Header("Fog References")]
    public GameObject NorthFog;
    public GameObject SouthFog;
    public GameObject EastFog;
    public GameObject WestFog;
    public TMP_Text NorthText;
    public TMP_Text SouthText;
    public TMP_Text EastText;
    public TMP_Text WestText;

    private readonly struct EmotionInfo
    {
        public EmotionData.EmotionType Emotion { get; }
        public Color Color { get; }

        public EmotionInfo(EmotionData.EmotionType emotion, Color color)
        {
            Emotion = emotion;
            Color = color;
        }
    }

    private static readonly EmotionInfo[] Emotions =
    {
        new(
            EmotionData.EmotionType.Happiness,
            new Color(1f, 1f, 0f, 0.4f)
        ),
        new(
            EmotionData.EmotionType.Surprise,
            new Color(1f, 0.5f, 0f, 0.4f)
        ),
        new(
            EmotionData.EmotionType.Neutral,
            new Color(0.5f, 0.5f, 0.5f, 0.4f)
        ),
        new(
            EmotionData.EmotionType.Fear,
            new Color(0f, 0f, 1f, 0.4f)
        )
    };

    private static EmotionInfo GetRandomEmotion()
    {
        return Emotions[Random.Range(0, Emotions.Length)];
    }

    public void Setup(int x, int y)
    {
        EmotionInfo nordData = GetRandomEmotion();
        EmotionInfo southData = GetRandomEmotion();
        EmotionInfo eastData = GetRandomEmotion();
        EmotionInfo westData = GetRandomEmotion();

        Data = new RoomData
        {
            X = x,
            Z = y,
            NorthCondition = new EmotionCondition(nordData.Emotion),
            SouthCondition = new EmotionCondition(southData.Emotion),
            EastCondition = new EmotionCondition(eastData.Emotion),
            WestCondition = new EmotionCondition(westData.Emotion)
        };

        NorthFog.GetComponent<Renderer>().material.color = nordData.Color;
        SouthFog.GetComponent<Renderer>().material.color = southData.Color;
        EastFog.GetComponent<Renderer>().material.color = eastData.Color;
        WestFog.GetComponent<Renderer>().material.color = westData.Color;

        NorthText.text = nordData.Emotion.ToString();
        SouthText.text = southData.Emotion.ToString();
        EastText.text = eastData.Emotion.ToString();
        WestText.text = westData.Emotion.ToString();

        DisableBorders();
    }

    public void DisableBorders()
    {
        if (Data.X == 0 && WestFog != null)
        {
            WestFog.SetActive(false);
        }

        if (Data.X == 4 && EastFog != null)
        {
            EastFog.SetActive(false);
        }

        if (Data.Z == 0 && SouthFog != null)
        {
            SouthFog.SetActive(false);
        }

        if (Data.Z == 4 && NorthFog != null)
        {
            NorthFog.SetActive(false);
        }
    }

    public FogCondition GetCondition(Direction direction)
    {
        return direction switch
        {
            Direction.North => Data.NorthCondition,
            Direction.South => Data.SouthCondition,
            Direction.East => Data.EastCondition,
            Direction.West => Data.WestCondition,
        };
    }
}