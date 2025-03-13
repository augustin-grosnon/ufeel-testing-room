using UnityEngine;
using System.IO;

public class EmotionServerController : PythonServerControllerBase
{
    private string _scriptPath;
    protected override string ScriptPath
    {
        get
        {
            _scriptPath ??= Path.Combine(Application.dataPath, "../PythonServers/EmotionDetection/emotion_detection.py");
            return _scriptPath;
        }
    }

    protected override string ServerName => "Emotion";
}
