using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UFeel;
using UnityEngine;
using UnityEngine.UI;

public class LabyrinthManager : MonoBehaviour
{
    public static LabyrinthManager Instance { get; private set; }

    [Header("References")]
    public GameObject RoomPrefab;
    public GameObject PlayerObject;

    [SerializeField]
    private Text emotionDebugText;

    [Header("Labyrinth Settings")]
    public static int LabyrinthSize = 5;
    public float RoomSpacing = 25f;

    private RoomController[,] rooms;

    private int currentX;
    private int currentZ;

    private const float MaxEmotionValue = 3f;
    private static readonly Dictionary<EmotionData.EmotionType, float> _emotionLevels = new();

    private static WaitForSeconds _waitForSeconds5 = new(5f);
    private bool playerDidWin;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private async void Start()
    {
        GenerateRooms();

        foreach (EmotionInfo emotionInfo in EmotionInfo.Emotions)
        {
            _emotionLevels[emotionInfo.Emotion] = 0f;
        }

        await Task.Delay(5000).ConfigureAwait(true);

        UFeelAPI.StartEmotionDetection();
    }

    private void Update()
    {
        EmotionData.EmotionType? currentEmotion = UFeelAPI.CurrentEmotionsData?.DominantEmotion;

        if (currentEmotion == null)
            return;

        foreach (EmotionInfo emotionInfo in EmotionInfo.Emotions)
        {
            EmotionData.EmotionType emotion = emotionInfo.Emotion;

            _emotionLevels[emotion] =
                emotion == currentEmotion
                    ? Mathf.Min(
                        MaxEmotionValue,
                        _emotionLevels[emotion] + Time.deltaTime
                    )
                    : Mathf.Max(
                        0f,
                        _emotionLevels[emotion] - Time.deltaTime
                    );
        }

        UpdateEmotionDebugText(currentEmotion);
    }

    public static bool IsEmotionCharged(EmotionData.EmotionType emotion)
    {
        return _emotionLevels.TryGetValue(emotion, out float value)
            && value >= MaxEmotionValue;
    }

    public static float GetEmotionLevel(EmotionData.EmotionType emotion)
    {
        return _emotionLevels.TryGetValue(emotion, out float value)
            ? value
            : 0f;
    }

    private void UpdateEmotionDebugText(EmotionData.EmotionType? currentEmotion)
    {
        if (emotionDebugText == null)
            return;

        if (playerDidWin)
            return;

        System.Text.StringBuilder builder = new();

        foreach (EmotionInfo emotionInfo in EmotionInfo.Emotions)
        {
            EmotionData.EmotionType emotion = emotionInfo.Emotion;
            float value = GetEmotionLevel(emotion);

            string color = "white";

            if (emotion == currentEmotion)
            {
                color = value >= 2.5f ? "green" : "orange";
            }
            builder.Append(
                $"<color={color}>{emotion} ({value:F1})</color>    "
            );
        }

        emotionDebugText.text = builder.ToString();
    }

    private void GenerateRooms()
    {
        rooms = new RoomController[LabyrinthSize, LabyrinthSize];

        GameObject parent = new("GeneratedRooms");

        for (int x = 0; x < LabyrinthSize; x++)
        {
            for (int z = 0; z < LabyrinthSize; z++)
            {
                Vector3 position = new(
                    x * RoomSpacing,
                    0f,
                    z * RoomSpacing
                );

                GameObject roomObject = Instantiate(RoomPrefab, position, Quaternion.identity);

                roomObject.name = $"Room_{x}_{z}";
                roomObject.transform.SetParent(parent.transform);

                RoomController room = roomObject.GetComponent<RoomController>();

                room.Setup(x, z);

                rooms[x, z] = room;
            }
        }

        Debug.Log("All rooms generated.");
    }

    public void TryMove(RoomController room, Direction direction)
    {
        FogCondition condition = room.GetCondition(direction);

        if (playerDidWin)
        {
            return;
        }

        if (!condition.CanPass())
        {
            return;
        }

        int targetX = room.Data.X;
        int targetZ = room.Data.Z;

        switch (direction)
        {
            case Direction.North:
                targetZ++;
                break;

            case Direction.South:
                targetZ--;
                break;

            case Direction.East:
                targetX++;
                break;

            case Direction.West:
                targetX--;
                break;
        }

        if (targetX < 0 ||
            targetX >= LabyrinthSize ||
            targetZ < 0 ||
            targetZ >= LabyrinthSize)
        {
            Debug.Log("Cannot leave labyrinth.");
            return;
        }

        currentX = targetX;
        currentZ = targetZ;

        TeleportPlayer(currentX, currentZ, direction);

        Debug.Log($"Moved to ({currentX},{currentZ})");

        CheckVictory();
    }

    private void TeleportPlayer(int x, int z, Direction cameFrom)
    {
        Vector3 targetPosition = new(
            x * RoomSpacing,
            1f,
            z * RoomSpacing
        );

        switch (cameFrom)
        {
            case Direction.North:
                targetPosition.z -= 5f;
                break;

            case Direction.South:
                targetPosition.z += 5f;
                break;

            case Direction.East:
                targetPosition.x -= 5f;
                break;

            case Direction.West:
                targetPosition.x += 5f;
                break;
        }

        if (PlayerObject.TryGetComponent(out FirstPersonController firstPersonController))
        {
            firstPersonController.Controller.enabled = false;
            firstPersonController.transform.position = targetPosition;
            firstPersonController.Controller.enabled = true;
        }
    }

    private void CheckVictory()
    {
        if (currentX == LabyrinthSize - 1 && currentZ == LabyrinthSize - 1)
        {
            playerDidWin = true;

            emotionDebugText.text = "<color=green>You won! Congratulations</color>";

            StartCoroutine(VictoryRoutine());
        }
    }

    private static IEnumerator VictoryRoutine()
    {
        yield return _waitForSeconds5;

        PauseMenu.GoToLobby();
    }
}
