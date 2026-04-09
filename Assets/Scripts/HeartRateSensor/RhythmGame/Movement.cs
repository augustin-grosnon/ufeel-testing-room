using UnityEngine;

public class Movement : MonoBehaviour
{
    void Start()
    {}

    float limitRotation(float rotation, float incrementor)
    {
        if ((rotation <= 90f && incrementor == -1f) ||
            (rotation >= 270f && incrementor == 1f))
            return rotation;
        
        return rotation + incrementor;
        
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.D))
        {
            transform.rotation = Quaternion.Euler(0f, limitRotation(transform.localEulerAngles.y, -1f), 0f);
        }
        if (Input.GetKey(KeyCode.F))
        {
            transform.rotation = Quaternion.Euler(0f, limitRotation(transform.localEulerAngles.y, 1f), 0f);
        }
    }
}
