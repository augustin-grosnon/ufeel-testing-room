using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UFeel;
using UnityEngine;

public class SpeechManager : MonoBehaviour
{
    private static WaitForSeconds _waitForSeconds60 = new(60f);
    private static WaitForSeconds _waitForSeconds5 = new(5f);
    private static WaitForSeconds _waitForSeconds3 = new(3f);
    private static WaitForSeconds _waitForSeconds2 = new(2f);

    [Header("Scene References")]
    // private FirstPersonController _player;
    [SerializeField] private VoiceDoorController doorController;
    [SerializeField] private VoiceWindowController windowController;
    [SerializeField] private AudioSource radioAudio;
    [SerializeField] private AudioSource windAudio;
    [SerializeField] private AudioSource tvAudio;
    [SerializeField] private Light roomLight;
    [SerializeField] private Light dummyLight;
    [SerializeField] private Light blueLight;

    [Header("TV System")]
    [SerializeField] private Renderer tvRenderer;
    [SerializeField] private Material tvBlueMaterial;

    [Header("Scene Hints")]
    [SerializeField] private GameObject radioHintText;
    [SerializeField] private GameObject windowHintText;
    [SerializeField] private GameObject windowHintTextLeft;
    [SerializeField] private GameObject windowHintTextRight;
    [SerializeField] private GameObject tvHintText;
    [SerializeField] private GameObject bookHintText;

    [Header("End Game")]
    [SerializeField] private CanvasGroup endCanvasGroup;
    [SerializeField] private TextMeshProUGUI endText;
    [SerializeField] private float fadeDuration = 5f;

    [Header("Hints System")]
    [SerializeField] private CanvasGroup hintCanvasGroup;
    [SerializeField] private TextMeshProUGUI hintText;
    private Coroutine hintCoroutine;
    private Coroutine activeShowHintCoroutine;

    private enum EscapeStep
    {
        Light,
        Radio,
        Window,
        BlueLight,
        TV,
        Door,
        Finished
    }
    private EscapeStep currentStep = EscapeStep.Light;
    private string lastProcessedSpeech = string.Empty;
    private Coroutine radioLoopCoroutine;
    private static readonly Regex _regex = new(@"[.,\/#!$%\^&\*;:{}=\-_`~()?']");
    private static readonly Regex _regex2 = new(@"\b(le|la|les|l|un|une|des|du|de)\b");
    private static readonly Regex _regex3 = new(@"\s+");

    private void Start()
    {
        UFeelAPI.ToggleOffEverything();
        UFeelDebugHUD.Clear();

        UFeelAPI.StartSpeechDetection();
        UFeelAPI.Status();

        radioLoopCoroutine = StartCoroutine(PlayRadioWithDelay());
        LightStep();
    }

    private void LightStep()
    {
        currentStep = EscapeStep.Light;
        StartHintTimer("Dites: \n\"allume la lumière\"");
    }

    private void ExecuteLightAction()
    {
        KillHint();
        roomLight.enabled = true;
        dummyLight.enabled = true;
        radioHintText.SetActive(true);
        windowHintTextLeft.SetActive(true);
        windowHintTextRight.SetActive(true);

        currentStep = EscapeStep.Radio;
        StartHintTimer("Dites: \n\"éteins la radio\"");
    }

    private void ExecuteRadioAction()
    {
        KillHint();
        if (radioLoopCoroutine != null)
        {
            StopCoroutine(radioLoopCoroutine);
        }
        if (radioAudio != null)
            radioAudio.Stop();
        if (radioHintText != null)
            radioHintText.SetActive(false);

        currentStep = EscapeStep.Window;
        StartHintTimer("Dites: \n\"ferme la fenêtre\"");
    }

    private void ExecuteWindowAction()
    {
        KillHint();
        if (windowController != null)
        {
            windowController.CloseWindow();
            StartCoroutine(SlideWindowTexts(1.5f));
        }
        if (windowHintText != null)
        {
            StartCoroutine(FadeInText(windowHintText, 6f));
            windAudio.Stop();
        }

        currentStep = EscapeStep.BlueLight;
        StartHintTimer("Dites: \n\"lumière violette\"");
    }

    private void ExecuteBlueLightAction()
    {
        KillHint();
        if (blueLight != null && roomLight != null && dummyLight != null && bookHintText != null)
        {
            roomLight.enabled = false;
            dummyLight.enabled = false;
            blueLight.enabled = true;
            windowHintText.SetActive(false);
            StartCoroutine(FadeInText(bookHintText, 3f));
        }

        currentStep = EscapeStep.TV;
        StartHintTimer("Dites: \n\"allume l'écran\"");
    }

    private void ExecuteTvAction()
    {
        KillHint();
        blueLight.enabled = false;
        bookHintText.SetActive(false);
        if (tvRenderer != null && tvBlueMaterial != null) StartCoroutine(TvOn(1.5f));
        if (tvHintText != null) StartCoroutine(FadeInText(tvHintText, 3f));

        currentStep = EscapeStep.Door;
        StartHintTimer("Dites: \n\"ouvre la porte\"");
    }

    private void ExecuteDoorAction()
    {
        KillHint();
        if (doorController != null) doorController.OpenDoor();
        roomLight.enabled = true;
        currentStep = EscapeStep.Finished;
        StartCoroutine(ShowEndScreen());
    }

    private void Update()
    {
        string currentSpeech = UFeelAPI.CurrentSpeech;

        if (string.IsNullOrEmpty(currentSpeech) || currentSpeech == lastProcessedSpeech) return;
        bool matchFound = false;

        switch (currentStep)
        {
            case EscapeStep.Light:
                if (IsSpeechMatch(currentSpeech, "allume la lumière", 0.75f))
                {
                    ExecuteLightAction();
                    matchFound = true;
                }
                break;
            case EscapeStep.Radio:
                if (IsSpeechMatch(currentSpeech, "éteins la radio", 0.75f))
                {
                    ExecuteRadioAction();
                    matchFound = true;
                }
                break;
            case EscapeStep.Window:
                if (IsSpeechMatch(currentSpeech, "ferme la fenêtre", 0.75f))
                {
                    ExecuteWindowAction();
                    matchFound = true;
                }
                break;
            case EscapeStep.BlueLight:
                if (IsSpeechMatch(currentSpeech, "lumière violette", 0.75f))
                {
                    ExecuteBlueLightAction();
                    matchFound = true;
                }
                break;
            case EscapeStep.TV:
                if (IsSpeechMatch(currentSpeech, "allume l'écran", 0.75f))
                {
                    ExecuteTvAction();
                    matchFound = true;
                }
                break;
            case EscapeStep.Door:
                if (IsSpeechMatch(currentSpeech, "ouvre la porte", 0.75f))
                {
                    ExecuteDoorAction();
                    matchFound = true;
                }
                break;
        }

        if (matchFound)
        {
            lastProcessedSpeech = currentSpeech;
        }
    }

    private void StartHintTimer(string command)
    {
        if (hintCoroutine != null)
            StopCoroutine(hintCoroutine);

        hintCoroutine = StartCoroutine(HintTimerCoroutine(command));
    }

    private void KillHint()
    {
        if (hintCoroutine != null)
        {
            StopCoroutine(hintCoroutine);
            hintCoroutine = null;
        }

        if (activeShowHintCoroutine != null)
        {
            StopCoroutine(activeShowHintCoroutine);
            activeShowHintCoroutine = null;
        }

        hintCanvasGroup.alpha = 0f;
        hintCanvasGroup.gameObject.SetActive(false);
        hintText.text = string.Empty;
    }

    private IEnumerator ShowHint(string text)
    {
        hintText.text = text;
        hintCanvasGroup.alpha = 1f;
        hintCanvasGroup.gameObject.SetActive(true);

        yield break;
    }

    private IEnumerator HintTimerCoroutine(string command)
    {
        yield return _waitForSeconds60;
        activeShowHintCoroutine = StartCoroutine(ShowHint(command));
    }

    private static IEnumerator FadeInText(GameObject textObject, float duration)
    {
        if (textObject == null) yield break;

        yield return _waitForSeconds3;

        if (!textObject.TryGetComponent(out TextMeshPro tmp)) yield break;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            tmp.alpha = t / duration;
            yield return null;
        }
        tmp.alpha = 1f;
    }

    private IEnumerator SlideWindowTexts(float duration)
    {
        Vector3 leftStart = windowHintTextLeft.transform.position;
        Vector3 rightStart = windowHintTextRight.transform.position;
        Vector3 leftTarget = new(22.16f, leftStart.y, leftStart.z);
        Vector3 rightTarget = new(27.14f, rightStart.y, rightStart.z);

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            windowHintTextLeft.transform.position = Vector3.Lerp(leftStart, leftTarget, t / duration);
            windowHintTextRight.transform.position = Vector3.Lerp(rightStart, rightTarget, t / duration);
            yield return null;
        }
        windowHintTextLeft.SetActive(false);
        windowHintTextRight.SetActive(false);
    }

    private IEnumerator TvOn(float duration)
    {
        if (tvRenderer == null) yield break;

        Material mat = tvRenderer.material;
        mat.EnableKeyword("_EMISSION");
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            Color c = Color.Lerp(Color.black, Color.white, t / duration);
            mat.SetColor("_BaseColor", c);
            mat.SetColor("_EmissionColor", c * 3f);
            yield return null;
        }
        if (tvAudio != null)
        {
            tvAudio.Play();
        }
    }

    private IEnumerator ShowEndScreen()
    {
        yield return _waitForSeconds2;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            endCanvasGroup.alpha = t / fadeDuration;
            yield return null;
        }

        yield return _waitForSeconds5;
        PauseMenu.GoToLobby();
    }

    private IEnumerator PlayRadioWithDelay()
    {
        while (currentStep is EscapeStep.Light or EscapeStep.Radio)
        {
            if (radioAudio != null)
            {
                radioAudio.Play();
                yield return new WaitForSeconds(radioAudio.clip.length);
            }
            yield return _waitForSeconds5;
        }
    }

    // private void OnDestroy()
    // {
    //     UFeelDebugHUD.UseDefaultDebugHUD = true;
    // }

    // Levenshtein Algo
    private static bool IsSpeechMatch(string currentText, string targetText, float thresholdPercent = 0.75f)
    {
        if (string.IsNullOrEmpty(currentText) || string.IsNullOrEmpty(targetText)) return false;

        string cleanSpoken = CleanText(currentText);
        string cleanTarget = CleanText(targetText);

        if (cleanSpoken == cleanTarget) return true;

        int n = cleanSpoken.Length;
        int m = cleanTarget.Length;
        int[,] d = new int[n + 1, m + 1];

        for (int i = 0; i <= n; d[i, 0] = i++)
        {
        }

        for (int j = 0; j <= m; d[0, j] = j++)
        {
        }

        for (int i = 1; i <= n; i++)
        {
            for (int j = 1; j <= m; j++)
            {
                int cost = (cleanTarget[j - 1] == cleanSpoken[i - 1]) ? 0 : 1;
                d[i, j] = Mathf.Min(
                    Mathf.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost
                );
            }
        }

        int distance = d[n, m];
        int maxLength = Mathf.Max(n, m);

        float similarity = 1.0f - ((float)distance / maxLength);
        return similarity >= thresholdPercent;
    }

    private static string CleanText(string text)
    {
        string t = text.ToLower().Trim();

        t = _regex.Replace(t, " ");
        t = _regex2.Replace(t, string.Empty);
        t = _regex3.Replace(t, " ").Trim();
        return t;
    }
}
