using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public abstract class PythonServerControllerBase : MonoBehaviour
{
    protected Process pythonProcess;
    [SerializeField] protected string pythonExecutable = "python3";

    protected abstract string ScriptPath { get; }

    protected abstract string ServerName { get; }

    public bool IsServerRunning => pythonProcess != null && !pythonProcess.HasExited;

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
                FileName = pythonExecutable,
                Arguments = ScriptPath,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            pythonProcess = Process.Start(psi);

            pythonProcess.OutputDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                    Debug.Log($"{ServerName} Python output: {args.Data}");
            };
            pythonProcess.ErrorDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                    Debug.LogError($"{ServerName} Python error: {args.Data}");
            };

            pythonProcess.BeginOutputReadLine();
            pythonProcess.BeginErrorReadLine();

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
                pythonProcess.Kill();
                pythonProcess.WaitForExit();
                pythonProcess = null;
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
    }
}
