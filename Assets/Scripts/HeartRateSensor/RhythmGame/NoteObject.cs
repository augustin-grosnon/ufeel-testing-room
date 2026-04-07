using UnityEngine;

public class NoteObject : MonoBehaviour
{
	public KeyCode keyToPress;
    private bool canBePressed = false;
	private float[] orientations = {180f, 90f, -90f, 0f};
	private KeyCode[] keys = {KeyCode.LeftArrow, KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.RightArrow};

    void Start()
    {}

	void ManageNote(int randomX, bool hit)
	{
		if (!hit && canBePressed) {
            GameManager.instance.NoteMissed();
			canBePressed = false;
		}
		
		transform.position = new Vector3(-1.5f + randomX, 10.5f, 0f);
		transform.rotation = Quaternion.Euler(0f, 0f, orientations[randomX]);
		keyToPress = keys[randomX];
	}

    void Update()
    {
        if (Input.GetKeyDown(keyToPress) && canBePressed)
        {
            GameManager.instance.NoteHit();
			canBePressed = false;
			ManageNote(Random.Range(0, 4), true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Activator")
        {
            canBePressed = true;
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (gameObject.activeInHierarchy)
		{
			if (collision.tag == "Activator")
        	{
				ManageNote(Random.Range(0, 4), false);
        	}
		}
    }
}
