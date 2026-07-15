using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Threading.Tasks;
using UFeel;

public class GameManager : MonoBehaviour
{
    private static WaitForSeconds _waitForSeconds1 = new WaitForSeconds(1f);
    public static GameManager Instance;

    public bool StartPlaying;
    public BeatScroller bs;

    public int CurrentScore;
    public int ScorePerNote = 100;

    public Text ScoreText;
    public Text MultiplierText;

    public int CurrentMultiplier = 1;
    public int MultiplierTracker = 0;
    public int MultiplierThreshold = 4;

    private const int MinBpm = 60;
    private const int MaxBpm = 180;
    private Coroutine actionCoroutine;
    private Coroutine actionCoroutine2;

    private bool simulated = false;
    private const int HpGainScore = 1;
    public Slider _slider;

    public AudioSource _music;
    public AudioSource _ECGBeep;

    private void Start()
    {
        Instance = this;
        _music.volume = 0.5f;
        _ECGBeep.volume = 0.25f;

        UFeelAPI.ToggleOffEverything();
        UFeelDebugHUD.Clear();

        InitializeDetection();

        actionCoroutine = StartCoroutine(UpdateBPM());
        actionCoroutine2 = StartCoroutine(PlayECGBeep());
    }

    private static void InitializeDetection()
    {
        UFeelAPI.StartHeartRateDetection();
        UFeelAPI.Status();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.V))
        {
            InitializeDetection();
        }
        if (Input.GetKeyUp(KeyCode.N))
        {
            simulated = !simulated;
            UFeelAPI.ToggleHeartRateSimulation(simulated);
        }
        if (!StartPlaying && Input.anyKeyDown)
        {
            StartPlaying = true;
            bs.Started = true;
            _music.Play();
        }
        if (_slider.value <= 0)
        {
            // DISPLAY GAME OVER SCREEN
        }
    }

    public void NoteHit()
    {
        MultiplierTracker++;

        if (MultiplierTracker >= MultiplierThreshold)
        {
            MultiplierTracker = 0;
            MultiplierThreshold *= 2;
            CurrentMultiplier++;
        }

        CurrentScore += ScorePerNote * CurrentMultiplier;
        ScoreText.text = "Score: " + CurrentScore;
        MultiplierText.text = "Multiplier: x" + CurrentMultiplier;
        _slider.value = Mathf.Clamp(_slider.value + (HpGainScore * CurrentMultiplier), _slider.minValue, _slider.maxValue);
    }

    public void NoteMissed()
    {
        CurrentMultiplier = 1;
        MultiplierTracker = 0;
        MultiplierThreshold = 4;
        MultiplierText.text = "Multiplier: x" + CurrentMultiplier;
        _slider.value = Mathf.Clamp(_slider.value - 5, _slider.minValue, _slider.maxValue);
    }

    private IEnumerator UpdateBPM()
    {
        while (true)
        {
            int bpm = GetBPM();

            if (bpm == 0)
                yield break;

            bs.Bpm = bpm;
            _music.pitch = bpm / (float)MinBpm;

            yield return _waitForSeconds1;
        }
    }

    private IEnumerator PlayECGBeep()
    {
        while (true)
        {
            _ECGBeep.Play();

            yield return new WaitForSeconds(1f / (bs.Bpm / 60f));
        }
    }

    private static int GetBPM()
    {
        int currentBPM = UFeelAPI.CurrentHeartRate ?? 0;

        if (currentBPM == 0)
            return currentBPM;

        return Mathf.Clamp(currentBPM, MinBpm, MaxBpm);
    }
}
