using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Threading.Tasks;
using UFeel;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public AudioSource audioSource;
    public Image redFilter;
    private int minBpm = 60;
    private int maxBpm = 180;
    private int bpm = 0;
    private Coroutine actionCoroutine;

    int getBPM()
    {
        int currentBPM = UFeelAPI.GetCurrentHeartRate() ?? 0;

        if (currentBPM == 0)
            return currentBPM;
        
        return Mathf.Clamp(currentBPM, minBpm, maxBpm);
    }
    
    async void Start()
    {
        await UFeelAPI.StartAPI();
        
        await Task.Delay(5000);
        
        UFeelAPI.StartHeartRateDetection();
        
        await Task.Delay(5000);

        bpm = getBPM();
        redFilter.color = new Color(1, 0, 0, 0);
        
        actionCoroutine = StartCoroutine(RunAudio());
    }
    
    IEnumerator RunAudio()
    {
        while (true)
        {
            bpm = getBPM();
            Debug.Log("BPM = " + bpm.ToString());
            
            if (bpm == 0)
                yield break;
            
            audioSource.Play();
            
            float newAlpha = redFilter.color.a == 0 ? 0.25f : 0;
            redFilter.color = new Color(1, 0, 0, newAlpha);

            Debug.Log(redFilter.color);
            
            float wait = 1.0f / (float)bpm * minBpm;
            yield return new WaitForSeconds(wait);
        }
    }
    
    void Update()
    {
        
    }
}
