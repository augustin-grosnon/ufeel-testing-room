using TMPro;
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

    public void Setup(int x, int y)
    {
        EmotionInfo nordData = EmotionInfo.GetRandomEmotion();
        EmotionInfo southData = EmotionInfo.GetRandomEmotion();
        EmotionInfo eastData = EmotionInfo.GetRandomEmotion();
        EmotionInfo westData = EmotionInfo.GetRandomEmotion();

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

        if (Data.X == LabyrinthManager.LabyrinthSize - 1 && EastFog != null)
        {
            EastFog.SetActive(false);
        }

        if (Data.Z == 0 && SouthFog != null)
        {
            SouthFog.SetActive(false);
        }

        if (Data.Z == LabyrinthManager.LabyrinthSize - 1 && NorthFog != null)
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