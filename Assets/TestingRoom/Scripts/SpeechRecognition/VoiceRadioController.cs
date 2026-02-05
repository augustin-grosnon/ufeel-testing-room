using UnityEngine;

public class VoiceRadioController : MonoBehaviour
{
    public AudioSource radioAudio;

    public void TurnOnRadio()
    {
        if (!radioAudio.isPlaying)
        {
            radioAudio.Play();
            Debug.Log("📻 Radio allumée");
        }
    }

    public void TurnOffRadio()
    {
        if (radioAudio.isPlaying)
        {
            radioAudio.Stop();
            Debug.Log("📻 Radio éteinte");
        }
    }
}
