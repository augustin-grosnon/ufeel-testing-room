using UnityEngine;

public class BeatScroller : MonoBehaviour
{
    public float beatTempo;
    public bool started;
    
    void Start()
    {
        beatTempo = 2f; // 120 / 60f
    }

    void Update()
    {
        if (started)
        {
            transform.position -= new Vector3(0f, beatTempo * Time.deltaTime, 0f);
        }
    }
}
