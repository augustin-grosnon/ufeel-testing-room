using DoorScript;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private GameObject _door;

    private void Awake()
    {
        if (!_door.TryGetComponent<MeshRenderer>(out _))
        {
            Debug.LogWarning("MeshRenderer not found on DoorController GameObject!");
        }
    }

    public void ToggleDoor()
    {
        if (_door.TryGetComponent(out Door doorScript))
        {
            doorScript.ToggleDoor();
        }
    }

    public void SetDoorColor(Color color)
    {
        if (_door.TryGetComponent(out MeshRenderer meshRenderer))
        {
            if (meshRenderer == null) return;
            if (meshRenderer.sharedMaterial == null) return;

            meshRenderer.sharedMaterial = new Material(meshRenderer.sharedMaterial)
            {
                color = color
            };
        }
    }

    public Color GetDoorColor()
    {
        if (!_door.TryGetComponent(out MeshRenderer meshRenderer))
        {
            Debug.LogWarning("MeshRenderer not found on Door GameObject!");
            return Color.white;
        }

        if (meshRenderer.sharedMaterial == null)
        {
            Debug.LogWarning("Door material is null!");
            return Color.white;
        }

        return meshRenderer.sharedMaterial.color;
    }
}

// TODO: update to directly retrieve the door from the attached object
