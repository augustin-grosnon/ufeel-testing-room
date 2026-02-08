using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private GameObject _door;

    void Awake()
    {
        MeshRenderer _meshRenderer = _door.GetComponent<MeshRenderer>();

        if (_meshRenderer == null)
        {
            Debug.LogWarning("MeshRenderer not found on DoorController GameObject!");
        }
    }

    public void ToggleDoor()
    {
        if (_door.TryGetComponent<DoorScript.Door>(out var doorScript))
        {
            doorScript.ToggleDoor();
        }
    }

    public void SetDoorColor(Color color)
    {
        if (_door.TryGetComponent<MeshRenderer>(out var _meshRenderer))
        {
            if (_meshRenderer == null) return;
            if (_meshRenderer.sharedMaterial == null) return;

            var matInstance = new Material(_meshRenderer.sharedMaterial)
            {
                color = color
            };
            _meshRenderer.sharedMaterial = matInstance;
        }
    }
}

// TODO: update to directly retrieve the door from the attached object
