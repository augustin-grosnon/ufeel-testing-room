using UnityEngine;
using System.Diagnostics;
using System.IO;
using Debug = UnityEngine.Debug;

public class EyeTrackingServerController : MonoBehaviour
{
    private Process pythonProcess;
    [SerializeField] private string pythonExecutable = "python3";
    private readonly string scriptPath = Path.Combine(Application.dataPath, "../PythonServers/EyeTracking/eye_tracking.py");

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
                Arguments = scriptPath,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            pythonProcess = Process.Start(psi);

            pythonProcess.OutputDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                    Debug.Log("EyeTracking Python output: " + args.Data);
            };
            pythonProcess.ErrorDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                    Debug.LogError("EyeTracking Python error: " + args.Data);
            };

            pythonProcess.BeginOutputReadLine();
            pythonProcess.BeginErrorReadLine();

            Debug.Log("EyeTracking Python server started.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to start EyeTracking Python server: " + e.Message);
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
                Debug.Log("EyeTracking Python server stopped.");
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to stop EyeTracking Python server: " + e.Message);
            }
        }
    }

    void OnApplicationQuit()
    {
        StopServer();
    }
}
