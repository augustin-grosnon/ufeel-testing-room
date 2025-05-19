using System;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class EyeTrackingSaveObject
{
    public float[] left;
    public float[] right;
    public float[] up;
    public float[] down;
}

public class EyeTrackingSetup : MonoBehaviour
{
    private static readonly string[] Directions = {"left", "right", "up", "down"};
    private int _directionIndex = 0;
    private bool _displayStartupMessage = true;
    private static readonly EyeDirectionRatio[] EyeDirectionRatios = new EyeDirectionRatio[Directions.Length];

    public GameObject startupMessage;
    public Text noEyesDetectedMessage;
    public RawImage eyeDetection;
    public Texture normalEyeDetectionTexture;
    public Texture notEyeDetectionTexture;

    private static String GetDirectionMessage(int directionIndex)
    {
        String operation = directionIndex == Directions.Length - 1 ? "finish" : "continue";
        String directionName = (directionIndex <= 1 ? "to the " : "") + Directions[directionIndex];
        return "Look " + directionName + "\nPress Enter to " + operation;
    }
    
    void Awake()
    {
        var _ = EyeTrackingReceiver.Instance;
        eyeDetection.color = Color.red;
    }

    void Update()
    {
        noEyesDetectedMessage.gameObject.SetActive(EyeTrackingReceiver.Error.error == EyeTrackingError.NO_EYES_DETECTED);
        
        switch (EyeTrackingReceiver.Error.error)
        {
            case EyeTrackingError.NO_EYES_DETECTED:
                eyeDetection.texture = notEyeDetectionTexture;
                return;
            default:
                eyeDetection.texture = normalEyeDetectionTexture;
                break;
        }
        
        if (!Input.GetKeyDown(KeyCode.Return))
            return;
        if (_displayStartupMessage)
        {
            _displayStartupMessage = false;
            // TODO: use Text Object directly
            startupMessage.GetComponent<Text>().text = GetDirectionMessage(_directionIndex);
            return;
        }
        
        if (!_displayStartupMessage && _directionIndex < Directions.Length)
        {
            var data = EyeTrackingReceiver.CurrentEyeRatios;
            
            EyeDirectionRatios[_directionIndex] = new EyeDirectionRatio
            {
                horizontal = data.horizontal,
                vertical = data.vertical
            };
            _directionIndex += 1;

            if (_directionIndex < Directions.Length)
            {
                // TODO: use Text Object directly
                startupMessage.GetComponent<Text>().text = GetDirectionMessage(_directionIndex);
                return;
            }
            startupMessage.gameObject.SetActive(false);
            eyeDetection.gameObject.SetActive(false);

            var eyeTrackingSaveObject = new EyeTrackingSaveObject
            {
                left = new []{EyeDirectionRatios[0].horizontal, EyeDirectionRatios[0].vertical},
                right = new []{EyeDirectionRatios[1].horizontal, EyeDirectionRatios[1].vertical},
                up = new []{EyeDirectionRatios[2].horizontal, EyeDirectionRatios[2].vertical},
                down = new []{EyeDirectionRatios[3].horizontal, EyeDirectionRatios[3].vertical}
            };

            string fileName = Application.dataPath + @"/../PythonServers/eye_tracker_values.json";
            String jsonObj = JsonUtility.ToJson(eyeTrackingSaveObject, true);
            System.IO.File.WriteAllText(fileName, jsonObj);
        }
    }
}
