using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    private bool playerInRange = false;
    private DoorController _doorController;
    // [SerializeField] private DoorIdentifier doorIdentifier;
    public string targetSceneName;

    void Start()
    {
        if (TryGetComponent<DoorController>(out var doorController))
        {
            _doorController = doorController;
            // _doorController.SetDoorColor(Color.red);
        }
        else
        {
            Debug.LogError("DoorController component is missing on this GameObject.");
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!string.IsNullOrEmpty(targetSceneName))
            {
                SceneLoader.Instance.LoadSceneAsync(targetSceneName, 3.0f);
                _doorController.ToggleDoor();
                // TODO: load scene only if the player enters the door
            }
            else
            {
                Debug.LogWarning("Target scene name is not set on DoorController.");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    void OnDrawGizmos()
    {
        if (TryGetComponent<BoxCollider>(out var box) && box.isTrigger)
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.25f);

            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(box.center, box.size);

            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}

// TODO: clean public -» check good practive between private SerializedField and public

// TODO: reorder architecture, put everything where it should be
