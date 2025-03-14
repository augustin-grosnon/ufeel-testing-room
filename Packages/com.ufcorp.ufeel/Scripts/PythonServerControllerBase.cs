using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public abstract class PythonServerControllerBase
{
    protected Process _pythonProcess;
    protected readonly string _pythonExecutable = "python3";

    protected PythonServerControllerBase()
    {
        Application.quitting += OnApplicationQuit;
    }

    protected abstract string ScriptPath { get; }

    protected abstract string ServerName { get; }

    public bool IsServerRunning => _pythonProcess != null && !_pythonProcess.HasExited;

    public void EnsureServerRunning()
    {
        if (!IsServerRunning)
        {
            StartServer();
        }
    }

    public void StartServer()
    {
        try
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = _pythonExecutable,
                Arguments = ScriptPath,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            _pythonProcess = Process.Start(psi);

            _pythonProcess.OutputDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                    Debug.Log($"{ServerName} Python output: {args.Data}");
            };
            _pythonProcess.ErrorDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                    Debug.LogError($"{ServerName} Python error: {args.Data}");
            };

            _pythonProcess.BeginOutputReadLine();
            _pythonProcess.BeginErrorReadLine();

            Debug.Log($"{ServerName} Python server started.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to start {ServerName} Python server: {e.Message}");
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
                Debug.Log($"{ServerName} Python server stopped.");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to stop {ServerName} Python server: {e.Message}");
            }
        }
    }

    void OnApplicationQuit()
    {
        StopServer();
        Application.quitting -= OnApplicationQuit;
    }
}
