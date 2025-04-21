using System;
using UnityEngine;
using UnityEngine.UI;

public class EyeTrackingSetup : MonoBehaviour
{
    private static readonly string[] Directions = {"to the left", "to the right", "up", "down"};
    private int _directionIndex = 0;
    private bool _displayStartupMessage = true;
    private static readonly EyeDirectionRatio[] EyeDirectionRatios = new EyeDirectionRatio[Directions.Length];

    public GameObject startupMessage;

    private static String GetDirectionMessage(int directionIndex)
    {
        String operation = directionIndex == Directions.Length - 1 ? "finish" : "continue";
        return "Look " + Directions[directionIndex] + "\nPress Enter to " + operation;
    }
    
    void Awake()
    {
        var _ = EyeTrackingReceiver.Instance;
    }

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Return))
            return;
        if (_displayStartupMessage)
        {
            _displayStartupMessage = false;
            Text text = startupMessage.GetComponent<Text>();
            text.text = GetDirectionMessage(_directionIndex);
            return;
        }
        if (!_displayStartupMessage && _directionIndex < Directions.Length)
        {
            var data = EyeTrackingReceiver.CurrentEyeRatios;
            Text text = startupMessage.GetComponent<Text>();
            
            EyeDirectionRatios[_directionIndex] = new EyeDirectionRatio
            {
                horizontal = data.horizontal,
                vertical = data.vertical
            };
            _directionIndex += 1;

            if (_directionIndex < Directions.Length)
            {
                text.text = GetDirectionMessage(_directionIndex);
                return;
            }
            text.gameObject.SetActive(false);
            
            foreach(EyeDirectionRatio edr in EyeDirectionRatios)
                Debug.Log("horizontal = " + edr.horizontal + " ; vertical = " + edr.vertical);
        }
    }
}
