using UnityEngine;

public sealed class EyeTrackingManager : MonoBehaviour
{
    [SerializeField] private Transform _sphere;
    [SerializeField] private Vector3 _movementArea = new Vector3(5f, 5f, 0f);

    void Awake()
    {
        var _ = EyeTrackingReceiver.Instance;
    }

    void Update()
    {
        var data = EyeTrackingReceiver.CurrentEyeData;

        Vector2 direction = Vector2.zero;
        if (data.left)  direction.x -= 1;
        if (data.right) direction.x += 1;
        if (data.up)    direction.y += 1;
        if (data.down)  direction.y -= 1;

        Vector3 targetPos;
        if (direction == Vector2.zero || data.center)
        {
            targetPos = new Vector3(0, 0, _sphere.position.z);
        }
        else
        {
            direction.Normalize();
            targetPos = new Vector3(
                direction.x * _movementArea.x / 2,
                direction.y * _movementArea.y / 2,
                _sphere.position.z
            );
        }

        _sphere.position = Vector3.Lerp(_sphere.position, targetPos, Time.deltaTime * 5f);
    }
}
