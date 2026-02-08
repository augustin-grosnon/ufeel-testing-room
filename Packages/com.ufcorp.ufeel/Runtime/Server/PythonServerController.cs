using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class PythonServerController
{
    private static PythonServerController _instance;
    private static readonly string scriptPath = Application.dataPath + "/../PythonServer/main.py";

    public static PythonServerController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PythonServerController(scriptPath, "Server");
            }
            return _instance;
        }
    }

    private Process _pythonProcess = null;

    private readonly string _venvPath =
#if UNITY_STANDALONE_WIN
    Application.dataPath + "/../PythonServer/venv/Scripts/python.exe";
#else
    Application.dataPath + "/../PythonServer/venv/bin/python3";
#endif

    private readonly string _scriptPath;
    private readonly string _serverName;

    private PythonServerController(string scriptPath, string serverName)
    {
        _scriptPath = scriptPath;
        _serverName = serverName;
    }

    public bool IsServerRunning => _pythonProcess != null && !_pythonProcess.HasExited;

    public void EnsureServerRunning()
    {
        if (!IsServerRunning)
        {
            StartServer();
        }
    }

    private void StartServer()
    {
        try
        {
            ProcessStartInfo psi = new()
            {
                FileName = _venvPath,
                Arguments = _scriptPath,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            _pythonProcess = Process.Start(psi);

            if (_pythonProcess == null)
            {
                return;
            }
            _pythonProcess.OutputDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    Debug.Log($"{_serverName} Python output: {args.Data}");
                }
            };
            _pythonProcess.ErrorDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {

                    Debug.Log($"{_serverName} Python error: {args.Data}");
                }
            };

            _pythonProcess.BeginOutputReadLine();
            _pythonProcess.BeginErrorReadLine();

            Debug.Log($"{_serverName} Python server started.");
        }
        catch (System.Exception e)
        {
            Debug.Log($"Failed to start {_serverName} Python server: {e.Message}");
        }
    }

    public void StopServer()
    {
        Debug.Log("Stop Server");
        if (IsServerRunning)
        {
            try
            {
                if (_pythonProcess != null)
                {
                    _pythonProcess.Kill();
                    _pythonProcess.WaitForExit();
                    _pythonProcess = null;
                }
                Debug.Log($"{_serverName} Python server stopped.");

            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to stop {_serverName} Python server: {e.Message}");
            }
        }
    }
}
