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
    public float hintDuration = 5f;
    private Coroutine hintCoroutine;


    async void Start()
    {
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

    private IEnumerator ShowHint(string text)
    {
        Debug.Log("Showing hint: " + text); 
        hintText.text = text;
        hintCanvasGroup.alpha = 1f;
        hintCanvasGroup.gameObject.SetActive(true);

        yield return new WaitForSeconds(hintDuration);

        hintCanvasGroup.alpha = 0f;
        hintCanvasGroup.gameObject.SetActive(false);
    }

    private IEnumerator HintTimerCoroutine(string command)
    {
        yield return new WaitForSeconds(80f);

        hintCoroutine = StartCoroutine(ShowHint(command));
    }

    private void StartHintTimer(string command)
    {
        if (hintCoroutine != null)
            StopCoroutine(hintCoroutine);

        hintCoroutine = StartCoroutine(HintTimerCoroutine(command));
    }

    private void LightStep()
    {   
        StartHintTimer("Dites: \n\"allume la lumière\"");

        UFeelAPI.TriggerActionOnSpeechOnce("allume la lumière", () =>
        { 
            if (hintCoroutine != null)
                StopCoroutine(hintCoroutine);   
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
            if (hintCoroutine != null)
                StopCoroutine(hintCoroutine); 

            Debug.Log("Turn off the Radio");
            if (radioAudio != null)
                radioAudio.Stop();
            if (radioHintText != null)
                radioHintText.SetActive(false);

            WindowStep();
        });
    }

    IEnumerator FadeInText(GameObject textObject, float duration)
    {
        if (textObject == null)
            yield break;

        yield return new WaitForSeconds(3f);

        TextMeshPro tmp = textObject.GetComponent<TextMeshPro>();

        if (tmp == null)
            yield break;

        Color color = tmp.color;
        float time = 0f;

        while (time < duration)
        {
            float alpha = Mathf.Lerp(0f, 1f, time / duration);
            tmp.color = new Color(color.r, color.g, color.b, alpha);

            time += Time.deltaTime;
            yield return null;
        }

        tmp.color = new Color(color.r, color.g, color.b, 1f);
    }

    IEnumerator SlideWindowTexts(float duration)
    {
        Vector3 leftStart = windowHintTextLeft.transform.position;
        Vector3 rightStart = windowHintTextRight.transform.position;

        Vector3 leftTarget = new Vector3(22.16f, leftStart.y, leftStart.z);
        Vector3 rightTarget = new Vector3(27.14f, rightStart.y, rightStart.z);

        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;

            windowHintTextLeft.transform.position = Vector3.Lerp(leftStart, leftTarget, t);
            windowHintTextRight.transform.position = Vector3.Lerp(rightStart, rightTarget, t);

            time += Time.deltaTime;
            yield return null;
        }

        windowHintTextLeft.SetActive(false);
        windowHintTextRight.SetActive(false);
    }
    private void WindowStep()
    {
        StartHintTimer("Dites: \n\"ferme la fenêtre\"");

        UFeelAPI.TriggerActionOnSpeechOnce("ferme la fenêtre", () =>
        { 
            if (hintCoroutine != null)
                StopCoroutine(hintCoroutine); 

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
    

    IEnumerator TvOn(float duration)
    {
        if (tvRenderer == null)
            yield break;

        Material mat = tvRenderer.material;

        Color startColor = mat.GetColor("_BaseColor");
        Color targetColor = Color.white;

        mat.EnableKeyword("_EMISSION");

        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;

            Color current = Color.Lerp(startColor, targetColor, t);

            mat.SetColor("_BaseColor", current);
            mat.SetColor("_EmissionColor", current * 3f);

            time += Time.deltaTime;
            yield return null;
        }

        tvAudio.Play();
        mat.SetColor("_BaseColor", targetColor);
        mat.SetColor("_EmissionColor", targetColor * 3f);
    }

    private void TvStep()
    {
        StartHintTimer("Dites: \n\"allume l'écran\"");

        UFeelAPI.TriggerActionOnSpeechOnce("allume l'écran", () =>
        {
            if (hintCoroutine != null)
                StopCoroutine(hintCoroutine); 

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

    private IEnumerator ShowEndScreen()
    {
        float t = 0f;

        yield return new WaitForSeconds(2f);
    
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = t / fadeDuration;
    
            endCanvasGroup.alpha = alpha;
    
            yield return null;
        }
    
        yield return new WaitForSeconds(2f);
        Application.Quit();
    }
    
    private void DoorStep()
    {
        StartHintTimer("Dites: \n\"ouvre la porte\"");
        UFeelAPI.TriggerActionOnSpeechOnce("ouvre la porte", () =>
        {
            if (hintCoroutine != null)
                StopCoroutine(hintCoroutine); 

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
}