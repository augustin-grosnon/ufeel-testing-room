using UnityEngine;
using UFeel;
using System.Threading.Tasks;
using TMPro;
using System.Collections;
using System.Reflection;
using UnityEngine.UI;


public class SpeechManager : MonoBehaviour
{
    [Header("Scene References")]
    private FirstPersonController _player;
    public VoiceDoorController doorController;
    public VoiceWindowController windowController;
    public AudioSource radioAudio;
    public AudioSource windAudio;
    public AudioSource tvAudio;
    public Light roomLight;
    public Light dummyLight;
    public Light blueLight;


    [Header("TV System")]
    public Renderer tvRenderer;
    public Material tvBlueMaterial;


    [Header("Scene Hints")]
    public GameObject radioHintText;
    public GameObject windowHintText;
    public GameObject windowHintTextLeft;
    public GameObject windowHintTextRight;
    public GameObject tvHintText;
    public GameObject bookHintText;


    [Header("End Game")]
    public CanvasGroup endCanvasGroup;
    public TextMeshProUGUI endText;
    public float fadeDuration = 5f;

    [Header("Hints System")]
    public CanvasGroup hintCanvasGroup;
    public TextMeshProUGUI hintText;
    private Coroutine hintCoroutine;
    private Coroutine activeShowHintCoroutine;


    async void Start()
    {
        UFeelDebugHUD.UseDefaultDebugHUD = false;
        UFeelDebugHUD.Clear();
        UFeelDebugHUD.Set("Current Speech", () =>  UFeelAPI.GetCurrentSpeech());


        await UFeelAPI.StartAPI();
        await Task.Delay(10000);

        UFeelAPI.StartSpeechDetection();
        UFeelAPI.Status();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            _player = playerObject.GetComponent<FirstPersonController>();
        }
        
        LightStep();
    }

    private void LightStep()
    {   
        StartHintTimer("Dites: \n\"allume la lumière\"");

        UFeelAPI.TriggerActionOnSpeechOnce("allume la lumière", () =>
        { 
            KillHint();
            Debug.Log("Executing light on command.");
            roomLight.enabled = true;
            dummyLight.enabled = true;
            
            radioHintText.SetActive(true);
            windowHintTextLeft.SetActive(true);
            windowHintTextRight.SetActive(true);

            RadioStep();
        });
    }

    private void RadioStep()
    {
        StartHintTimer("Dites: \n\"éteins la radio\"");

        UFeelAPI.TriggerActionOnSpeechOnce("éteins la radio", () =>
        { 
            KillHint();

            Debug.Log("Turn off the Radio");
            if (radioAudio != null)
                radioAudio.Stop();
            if (radioHintText != null)
                radioHintText.SetActive(false);

            WindowStep();
        });
    }

    private void WindowStep()
    {
        StartHintTimer("Dites: \n\"ferme la fenêtre\"");

        UFeelAPI.TriggerActionOnSpeechOnce("ferme la fenêtre", () =>
        { 
            KillHint();

            Debug.Log("Executing shutter close command.");
            radioAudio.Stop();
            if (windowController != null) {
                windowController.CloseWindow();
                StartCoroutine(SlideWindowTexts(1.5f));
            }

            if (windowHintText != null) {
                StartCoroutine(FadeInText(windowHintText, 6f));
                windAudio.Stop();
            }

            blueLightStep();
            
        });
    }

    private void blueLightStep()
    {
        StartHintTimer("Dites: \n\"lumière violette\"");

        UFeelAPI.TriggerActionOnSpeechOnce("lumière violette", () =>
        {
            KillHint();

            Debug.Log("Executing light blue command.");
            if (blueLight != null && roomLight != null && dummyLight != null && bookHintText != null) {
                roomLight.enabled = false;
                dummyLight.enabled = false;
                blueLight.enabled = true;
                windowHintText.SetActive(false);
                StartCoroutine(FadeInText(bookHintText, 3f));
            }

            TvStep();
        });
    }
    
    private void TvStep()
    {
        StartHintTimer("Dites: \n\"allume l'écran\"");

        UFeelAPI.TriggerActionOnSpeechOnce("allume l'écran", () =>
        {
            KillHint();

            Debug.Log("Executing tv on command.");
            blueLight.enabled = false;
            bookHintText.SetActive(false);

            if (tvRenderer != null && tvBlueMaterial != null)
                StartCoroutine(TvOn(1.5f));
            
            if (tvHintText != null)
                StartCoroutine(FadeInText(tvHintText, 3f));

            DoorStep();
        });
    }
    
    private void DoorStep()
    {
        StartHintTimer("Dites: \n\"ouvre la porte\"");
        UFeelAPI.TriggerActionOnSpeechOnce("ouvre la porte", () =>
        {
            KillHint();

            Debug.Log("Executing door open command.");
            if (doorController != null)
                doorController.OpenDoor();
            roomLight.enabled = true;

            StartCoroutine(ShowEndScreen());
        });
    }

    void Update()
    {
        return;
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
        hintText.text = "";
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
        yield return new WaitForSeconds(60f);
        activeShowHintCoroutine = StartCoroutine(ShowHint(command));
    }

    IEnumerator FadeInText(GameObject textObject, float duration)
    {
        if (textObject == null) yield break;

        yield return new WaitForSeconds(3f);

        TextMeshPro tmp = textObject.GetComponent<TextMeshPro>();

        if (tmp == null) yield break;

        for (float t = 0; t < duration; t += Time.deltaTime) {
            tmp.alpha = t / duration;
            yield return null;
        }
        tmp.alpha = 1f;
    }

    IEnumerator SlideWindowTexts(float duration)
    {
        Vector3 leftStart = windowHintTextLeft.transform.position;
        Vector3 rightStart = windowHintTextRight.transform.position;
        Vector3 leftTarget = new Vector3(22.16f, leftStart.y, leftStart.z);
        Vector3 rightTarget = new Vector3(27.14f, rightStart.y, rightStart.z);

        for (float t = 0; t < duration; t += Time.deltaTime) {
            windowHintTextLeft.transform.position = Vector3.Lerp(leftStart, leftTarget, t / duration);
            windowHintTextRight.transform.position = Vector3.Lerp(rightStart, rightTarget, t / duration);
            yield return null;
        }
        windowHintTextLeft.SetActive(false);
        windowHintTextRight.SetActive(false);
    }

    IEnumerator TvOn(float duration)
    {
        if (tvRenderer == null) yield break;

        Material mat = tvRenderer.material;
        mat.EnableKeyword("_EMISSION");
        for (float t = 0; t < duration; t += Time.deltaTime) {
            Color c = Color.Lerp(Color.black, Color.white, t / duration);
            mat.SetColor("_BaseColor", c);
            mat.SetColor("_EmissionColor", c * 3f);
            yield return null;
        }
        tvAudio?.Play();
    }

    private IEnumerator ShowEndScreen()
    {
        yield return new WaitForSeconds(2f);
        for (float t = 0; t < fadeDuration; t += Time.deltaTime) {
            endCanvasGroup.alpha = t / fadeDuration;
            yield return null;
        }

        yield return new WaitForSeconds(5f);
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    void OnDestroy()
    {
        UFeelDebugHUD.UseDefaultDebugHUD = true;
    }
}