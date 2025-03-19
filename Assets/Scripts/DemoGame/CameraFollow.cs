using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform _player;
    public Vector3 _offset;

    void Update()
    {
        transform.position = _player.position + _offset;
    }
}
