using UnityEngine;
using System.Diagnostics;
using System.Threading;
using Debug = UnityEngine.Debug;

public class PythonServerController
{
    private static PythonServerController _instance;
    private static SynchronizationContext _mainContext;
    // TODO: Add centralized logging queue later if needed

    public static PythonServerController Instance
    {
        get
        {
            if (_instance == null)
            {
                string scriptPath = Application.dataPath + "/../PythonServers/main.py";
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

        _mainContext = SynchronizationContext.Current ?? new SynchronizationContext();
        Application.quitting += OnApplicationQuit;
    }

    public bool IsServerRunning => _pythonProcess != null && !_pythonProcess.HasExited;

    public void EnsureServerRunning()
    {
        if (!IsServerRunning)
        {
            Thread serverThread = new(StartServer)
            {
                IsBackground = true
            };
            serverThread.Start();
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
                {
                    _Log($"{_serverName} Python output: {args.Data}");
                }
            };
            _pythonProcess.ErrorDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {

                    _Log($"{_serverName} Python error: {args.Data}", true);
                }
            };

            _pythonProcess.BeginOutputReadLine();
            _pythonProcess.BeginErrorReadLine();

            _Log($"{_serverName} Python server started.");
        }
        catch (System.Exception e)
        {
            _Log($"Failed to start {_serverName} Python server: {e.Message}", true);
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

                _Log($"{_serverName} Python server stopped.");
            }
            catch (System.Exception e)
            {
                _Log($"Failed to stop {_serverName} Python server: {e.Message}", true);
            }
        }
    }

    private void OnApplicationQuit()
    {
        StopServer();
        Application.quitting -= OnApplicationQuit;
    }

    private void _Log(string message, bool isError = false)
    {
        _mainContext.Post(_ =>
        {
            if (isError)
                Debug.LogError(message);
            else
                Debug.Log(message);
        }, null);
    }
}

// TODO: check if it causes any issue
