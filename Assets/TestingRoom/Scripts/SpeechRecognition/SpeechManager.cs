// using System.Diagnostics;
using UnityEngine;

public class SpeechManager : MonoBehaviour
{

    private FirstPersonController _player;
    private string lastCommand = "";

    public Light lightBulb;
    public VoiceDoorController doorController;
    public VoiceWindowController windowController;
    public AudioSource radioAudio;

    void Awake()
    {
        var _ = SpeechToTextReceiver.Instance;
    }

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            _player = playerObject.GetComponent<FirstPersonController>();
        }
    }

    void LateUpdate()
    {
        CheckVoiceCommand();

    }

    private string GetCurrentText()
    {
        return SpeechToTextReceiver.CurrentText.text;
    }

    private void GetNewText()
    {
        string command = GetCurrentText();

        if (command != lastCommand)
        {
            Debug.Log($"Nouveau texte reçu : {command}");
        }
    }

    private void changeColor(string command)
    {
        if (lightBulb.enabled == false)
            return;
    
        if (command.Contains("lumière rouge"))
        {
            lightBulb.color = Color.red;
        }
        else if (command.Contains("lumière bleue"))
        {
            lightBulb.color = Color.blue;
        }
        else if (command.Contains("lumière jaune"))
        {
            lightBulb.color = Color.yellow;
        }
        else if (command.Contains("lumière blanche"))
        {
            lightBulb.color = Color.white;
        }
    }

    private void CheckVoiceCommand()
    {
        string command = GetCurrentText();

        Debug.Log($"Received current text : {command}");

        if (string.IsNullOrEmpty(command))
            return;

        lastCommand = command;

        changeColor(command);

        if (_player == null)
        {
            Debug.LogWarning("FirstPersonController not found.");
            return;
        }

        if (command.Contains("jump"))
        {
            Debug.Log("Executing jump command.");
            _player.ExecuteJump();
        }

        if (command.Contains("allume la lumière"))
        {
            Debug.Log("Executing light on command.");
            if (lightBulb != null)
                lightBulb.enabled = true;
        }

        if (command.Contains("éteins la lumière"))
        {
            Debug.Log("Executing light off command.");
            if (lightBulb != null)
                lightBulb.enabled = false;
        }
            
        if (command.Contains("ouvre la porte"))
        {
            Debug.Log("Executing door open command.");
            if (doorController != null)
                doorController.OpenDoor();
        }

        if (command.Contains("ferme la porte"))
        {
            Debug.Log("Executing door close command.");
            if (doorController != null)
                doorController.CloseDoor();
        }

        if (command.Contains("ouvre la fenêtre"))
        {
            Debug.Log("Executing shutter open command.");
            if (windowController != null)
                windowController.OpenWindow();
        }

        if (command.Contains("ferme la fenêtre"))
        {
            Debug.Log("Executing shutter close command.");
            if (windowController != null)
                windowController.CloseWindow();
        }

        if (command.Contains("allume la radio") && !radioAudio.isPlaying)
        {
            Debug.Log("Turn on the Radio");
            radioAudio.Play();
        }

        if (command.Contains("éteins la radio") && radioAudio.isPlaying)
        {
            Debug.Log("Turn off the radio");
            radioAudio.Stop();
        }
    }
}