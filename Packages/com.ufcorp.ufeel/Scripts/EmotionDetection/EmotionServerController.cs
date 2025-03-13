using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.IO;

// TODO: check if the emotion controller should be a MonoBehaviour

public class EmotionServerController : MonoBehaviour
{
    private Process pythonProcess;
    [SerializeField] private string pythonExecutable = "python3";
    readonly string scriptPath = Path.Combine(Application.dataPath, "../PythonServers/EmotionDetection/emotion_detection.py");

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
            ProcessStartInfo psi = new()
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
                    Debug.Log("Python output: " + args.Data);
            };
            pythonProcess.ErrorDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                    Debug.LogError("Python error: " + args.Data);
            };

            pythonProcess.BeginOutputReadLine();
            pythonProcess.BeginErrorReadLine();

            Debug.Log("Python server started.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to start Python server: " + e.Message);
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
                Debug.Log("Python server stopped.");
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to stop Python server: " + e.Message);
            }
        }
    }

    void OnApplicationQuit()
    {
        StopServer();
    }
}
