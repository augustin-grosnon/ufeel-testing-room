// using System.Diagnostics;
using UnityEngine;

public class SpeechManager : MonoBehaviour
{

    private FirstPersonController _player;
    private string lastCommand = "";

    public Light lightBulb;
    public VoiceDoorController doorController;

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

    private void CheckVoiceCommand()
    {
        string command = GetCurrentText();

        Debug.Log($"Received current text : {command}");

        if (string.IsNullOrEmpty(command))
            return;

        lastCommand = command;

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
            {
                lightBulb.enabled = true;
                Debug.Log("💡 Lumière allumée !");
            }
        }

        if (command.Contains("éteins la lumière"))
        {
            Debug.Log("Executing light off command.");
            if (lightBulb != null)
            {
                lightBulb.enabled = false;
                Debug.Log("💡 Lumière éteinte !");
            }
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

        //SpeechToTextReceiver.CurrentText.text = "";
    }
}

    // Trouver le player via le tag, et appeler une methode publique sur son script de mouvement
    // ex: player.TryGetComponent<FirstPersonController>().ExecuteJump(force);