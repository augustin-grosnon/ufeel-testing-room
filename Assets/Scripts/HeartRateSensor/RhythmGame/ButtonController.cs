using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ButtonController : MonoBehaviour
{
    private SpriteRenderer sr;
    public Sprite image;
    public Sprite pressedImage;
    public KeyCode keyToPress;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(keyToPress))
        {
            sr.sprite = pressedImage;
        }

        if (Input.GetKeyUp(keyToPress))
        {
            sr.sprite = image;
        }
    }
}
