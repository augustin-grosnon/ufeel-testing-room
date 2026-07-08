using UnityEngine;

public class BeatScroller : MonoBehaviour
{
    public float beatTempo;
    public int bpm;
    public bool started;
    
    void Start()
    {
        beatTempo = 2;
        bpm = 60;
    }

    void Update()
    {
        if (started)
        {
            beatTempo = (float)bpm / 30f;
            transform.position -= new Vector3(0f, beatTempo * Time.deltaTime, 0f);
        }
    }
}
