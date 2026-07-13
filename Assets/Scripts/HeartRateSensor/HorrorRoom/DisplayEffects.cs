using System.Collections;
using UFeel;
using UnityEngine;
using UnityEngine.UI;

public class DisplayEffects : MonoBehaviour
{
    public AudioSource AudioSource;
    public Image RedFilter;
    private int minBpm = 60;
    private int maxBpm = 180;
    private int bpm = 0;
    private Coroutine actionCoroutine;

    private int GetBPM()
    {
        int currentBPM = UFeelAPI.CurrentHeartRate ?? 0;

        if (currentBPM == 0)
        {
            return currentBPM;
        }

        return Mathf.Clamp(currentBPM, minBpm, maxBpm);
    }

    private void Start()
    {
        UFeelAPI.ToggleOffEverything();
        UFeelDebugHUD.Clear();

        UFeelAPI.StartHeartRateDetection();

        bpm = GetBPM();
        RedFilter.color = new Color(1, 0, 0, 0);

        actionCoroutine = StartCoroutine(RunAudio());
    }

    private IEnumerator RunAudio()
    {
        while (true)
        {
            bpm = GetBPM();
            Debug.Log("BPM = " + bpm.ToString());

            if (bpm == 0)
                yield break;

            AudioSource.Play();

            float newAlpha = RedFilter.color.a == 0 ? 0.25f : 0;
            RedFilter.color = new Color(1, 0, 0, newAlpha);

            Debug.Log(RedFilter.color);

            float wait = 1.0f / bpm * minBpm;
            yield return new WaitForSeconds(wait);
        }
    }
}
