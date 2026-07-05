using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Threading.Tasks;
using UFeel;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

    public bool startPlaying;
    public BeatScroller bs;

	public int currentScore;
	public int scorePerNote = 100;

	public Text scoreText;
	public Text multiplierText;

    public int currentMultiplier = 1;
	public int multiplierTracker = 0;
	public int multiplierThreshold = 4;
	
	private const int minBpm = 60;
	private const int maxBpm = 180;
	private Coroutine actionCoroutine;

	private const int hpGainScore = 1;
	public Slider _slider;
    
    async void Start()
    {
        instance = this;
        await UFeelAPI.StartAPI();
        
        await Task.Delay(5000);
        
        UFeelAPI.StartHeartRateDetection();
        
        await Task.Delay(5000);
        
        actionCoroutine = StartCoroutine(UpdateBPM());
    }

    void Update()
    {
        if (!startPlaying)
        {
            if (Input.anyKeyDown)
            {
                startPlaying = true;
                bs.started = true;
            }
        }
		if (_slider.value <= 0) {
			// DISPLAY GAME OVER SCREEN
		}
    }

    public void NoteHit()
    {
		multiplierTracker++;

		if (multiplierTracker >= multiplierThreshold)
		{
			multiplierTracker = 0;
			multiplierThreshold *= 2;
			currentMultiplier++;
		}

		currentScore += scorePerNote * currentMultiplier;
		scoreText.text = "Score: " + currentScore;
		multiplierText.text = "Multiplier: x" + currentMultiplier;
		_slider.value = Mathf.Clamp(_slider.value + hpGainScore * currentMultiplier, _slider.minValue, _slider.maxValue);
    }

    public void NoteMissed()
    {
		currentMultiplier = 1;
		multiplierTracker = 0;
		multiplierThreshold = 4;
		multiplierText.text = "Multiplier: x" + currentMultiplier;
		_slider.value = Mathf.Clamp(_slider.value - 5, _slider.minValue, _slider.maxValue);
    }
    
    IEnumerator UpdateBPM()
    {
	    while (true)
	    {
		    int bpm = getBPM();
            
		    if (bpm == 0)
			    yield break;
		    
		    bs.beatTempo = bpm / 30f; // 60f / 2
		    
		    yield return new WaitForSeconds(1f);
	    }
    }
    
	int getBPM()
	{
	    int currentBPM = UFeelAPI.GetCurrentHeartRate() ?? 0;
	    
	    if (currentBPM == 0)
		    return currentBPM;
	    
	    return Mathf.Clamp(currentBPM, minBpm, maxBpm);
	}
}
