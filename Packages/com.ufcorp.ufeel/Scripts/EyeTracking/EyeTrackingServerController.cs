using UnityEngine;
using System.IO;

public class EyeTrackingServerController : PythonServerControllerBase
{
    private string _scriptPath;
    protected override string ScriptPath
    {
        get
        {
            _scriptPath ??= Path.Combine(Application.dataPath, "../PythonServers/EyeTracking/eye_tracking.py");
            return _scriptPath;
        }
    }

    protected override string ServerName => "EyeTracking";
}
