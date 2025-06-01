using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class VehicleController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float accelerationForce = 800f;
    // [SerializeField] private float steeringTorque = 300f;
    [SerializeField] private float steeringTorque = 30f;
    [SerializeField] private float maxSpeed = 20f;

    [Header("References")]
    [SerializeField] private Transform centerOfMass;

    private Rigidbody rb;
    private float throttleInput = 0f;
    private float steeringInput = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (centerOfMass != null)
            rb.centerOfMass = transform.InverseTransformPoint(centerOfMass.position);
    }

    void Update()
    {
        // TODO: replace with eye tracking handling

        var data = EyeTrackingReceiver.CurrentEyeData;
        if (data.left && !data.right)
            steeringInput = -0.3f;
        else if (data.right && !data.left)
            steeringInput = 0.3f;
        else
            steeringInput = 0f;

        throttleInput = Input.GetAxis("Vertical");
        // steeringInput = Input.GetAxis("Horizontal");
    }

    void FixedUpdate()
    {
        if (rb.linearVelocity.magnitude < maxSpeed)
        {
            rb.AddForce(accelerationForce * throttleInput * Time.fixedDeltaTime * transform.forward);
        }

        // rb.AddTorque(steeringInput * steeringTorque * Time.fixedDeltaTime * Vector3.up);

        Quaternion turnRotation = Quaternion.Euler(0f, steeringInput * steeringTorque * Time.fixedDeltaTime, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);
    }
}
