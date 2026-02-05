using UnityEngine;

public class VoiceWindowController : MonoBehaviour
{
    public Transform leftShutter;
    public Transform rightShutter;

    public float openLeft = -7.5f;
    public float openRight = 7f;

    public float closeLeft = -2.85f;
    public float closeRight = 2.15f;

    public float speed = 2f;

    private bool isOpen = false;

    public void OpenWindow()
    {
        isOpen = true;
    }

    public void CloseWindow()
    {
        isOpen = false;
    }

    void Update()
    {
        Vector3 leftPos = leftShutter.localPosition;
        Vector3 rightPos = rightShutter.localPosition;


        leftPos.x = Mathf.Lerp(leftPos.x, isOpen ? openLeft: closeLeft, Time.deltaTime * speed);

        rightPos.x = Mathf.Lerp( rightPos.x, isOpen ? openRight : closeRight, Time.deltaTime * speed);


        leftShutter.localPosition = leftPos;
        rightShutter.localPosition = rightPos;
    }
}
