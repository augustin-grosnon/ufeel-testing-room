using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class PythonServerController
{
    private static PythonServerController _instance;

    public static PythonServerController Instance
    {
        get
        {
            if (_instance == null)
            {
                string scriptPath = Application.dataPath + "/../PythonServers/main.py";
                // string scriptPath = "-c \"import cv2; print(cv2.__version__)\"";
                // string scriptPath = "--version";
                // string scriptPath = Application.dataPath + "/../PythonServers/debug.py";

                _instance = new PythonServerController(scriptPath, "Server");
            }

            return _instance;
        }
    }

    private Process _pythonProcess;
    private readonly string _pythonExecutable = "python3";
    private readonly string _scriptPath;
    private readonly string _serverName;

    private PythonServerController(string scriptPath, string serverName)
    {
        _scriptPath = scriptPath;
        _serverName = serverName;

        Application.quitting += OnApplicationQuit;
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
            ProcessStartInfo psi = new ProcessStartInfo()
            {
                FileName = _pythonExecutable,
                Arguments = _scriptPath,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            _pythonProcess = Process.Start(psi);

            _pythonProcess.OutputDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                    Debug.Log($"{_serverName} Python output: {args.Data}");
            };
            _pythonProcess.ErrorDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                    Debug.LogError($"{_serverName} Python error: {args.Data}");
            };

            _pythonProcess.BeginOutputReadLine();
            _pythonProcess.BeginErrorReadLine();

            Debug.Log($"{_serverName} Python server started.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to start {_serverName} Python server: {e.Message}");
        }
    }

    public void StopServer()
    {
        if (IsServerRunning)
        {
            try
            {
                _pythonProcess.Kill();
                _pythonProcess.WaitForExit();
                _pythonProcess = null;

                Debug.Log($"{_serverName} Python server stopped.");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to stop {_serverName} Python server: {e.Message}");
            }
        }
    }

    private void OnApplicationQuit()
    {
        StopServer();
        Application.quitting -= OnApplicationQuit;
    }
}
