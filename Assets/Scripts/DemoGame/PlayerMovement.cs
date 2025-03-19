using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float _speed = 4f;

    void Update()
    {
        float moveDirection = Input.GetAxis("Vertical") * _speed * Time.deltaTime;
        float strafeDirection = Input.GetAxis("Horizontal") * _speed * Time.deltaTime;

        transform.Translate(strafeDirection, 0, moveDirection);
    }
}
