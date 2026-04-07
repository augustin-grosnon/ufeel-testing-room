using UnityEngine;

public class ButtonController : MonoBehaviour
{
    private SpriteRenderer sr;
    public Sprite image;
    public Sprite pressedImage;
    public KeyCode keyToPress;
    
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
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
