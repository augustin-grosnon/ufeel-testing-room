using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    private bool playerInRange = false;
    private DoorController _doorController;
    public string targetSceneName;

    void Start()
    {
        if (TryGetComponent<DoorController>(out var doorController))
        {
            _doorController = doorController;
            _doorController.SetDoorColor(Color.red);
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
                SceneLoader.Instance.LoadSceneAsync(targetSceneName);
                _doorController.ToggleDoor();
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
}
