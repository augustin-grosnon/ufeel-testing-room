using UnityEngine;

public class FogTrigger : MonoBehaviour
{
    [Header("Fog Settings")]
    public Direction Direction;

    private RoomController room;

    private void Awake()
    {
        room = GetComponentInParent<RoomController>();

        if (room == null)
        {
            Debug.LogError(
                $"No RoomController found for fog {gameObject.name}"
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Adventurer"))
            return;

        if (room == null)
            return;

        LabyrinthManager.Instance.TryMove(room, Direction);
    }
}
