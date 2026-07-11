using UnityEngine;
using UnityEngine.Assertions.Must;

public class LabyrinthManager : MonoBehaviour
{
    public static LabyrinthManager Instance { get; private set; }

    [Header("References")]
    public GameObject RoomPrefab;
    private GameObject playerObject;

    [Header("Labyrinth Settings")]
    public int LabyrinthSize = 5;
    public float RoomSpacing = 25f;

    private RoomController[,] rooms;

    private RoomController currentRoom;

    private int currentX;
    private int currentZ;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        playerObject = GameObject.FindWithTag("Adventurer");
        if (playerObject)
        {
            Debug.Log("I Found the player");
        }
        else
        {
            Debug.Log("Didn't find any player");
        }
        GenerateRooms();
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

        currentRoom = rooms[currentX, currentZ];

        TeleportPlayer(currentX, currentZ, direction);

        Debug.Log($"Moved to ({currentX},{currentZ})");

        CheckVictory();
    }

    private void TeleportPlayer(int x, int z, Direction cameFrom)
    {
        // Vector3 targetPosition = default;

        // switch (cameFrom)
        // {
        //     case Direction.North:
        //         targetPosition.z = 10f;
        //         break;

        //     case Direction.South:
        //         targetPosition.z = -10f;
        //         break;

        //     case Direction.East:
        //         targetPosition.x = 10f;
        //         break;

        //     case Direction.West:
        //         targetPosition.x = -10f;
        //         break;
        // }

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

        if (playerObject.TryGetComponent(out FirstPersonController firstPersonController))
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
            Debug.Log("YOU ESCAPED !");
        }
    }
}
