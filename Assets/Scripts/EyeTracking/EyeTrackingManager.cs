using UnityEngine;

public sealed class EyeTrackingManager : MonoBehaviour
{
    [SerializeField] private Transform _sphere;
    [SerializeField] Vector3 _movementArea = new(5f, 5f, 0f);

    void Awake()
    {
        var _ = EyeTrackingReceiver.Instance;
    }

    void Update()
    {
        Vector2 eyePos = EyeTrackingReceiver.CurrentGaze;

        Vector3 targetPos = new(
            Mathf.Lerp(-_movementArea.x / 2, _movementArea.x / 2, eyePos.x),
            Mathf.Lerp(-_movementArea.y / 2, _movementArea.y / 2, eyePos.y),
            _sphere.position.z
        );

        _sphere.position = Vector3.Lerp(_sphere.position, targetPos, Time.deltaTime * 5f);
    }
}
