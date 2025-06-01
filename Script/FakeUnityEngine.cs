using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace UnityEngine
{
    // ✅ Debug
    public static class Debug
    {
        public static void Log(object message) => Console.WriteLine("[LOG] " + message);
        public static void LogWarning(object message) => Console.WriteLine("[WARN] " + message);
        public static void LogError(object message) => Console.Error.WriteLine("[ERROR] " + message);
    }

    // ✅ MonoBehaviour stub
    public class MonoBehaviour
    {

    }

    // ✅ JsonUtility (limited implementation)
    public static class JsonUtility
    {
        public static string ToJson(object obj, bool prettyPrint = false)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = prettyPrint,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            return JsonSerializer.Serialize(obj, options);
        }

        public static T? FromJson<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json);
        }
    }

    // ✅ Application (limited implementation)
    class Application
    {

        public static string dataPath = "PythonServers";
        public delegate void QuittingHandler();

        // Static event to add/remove quitting handlers
        public static event QuittingHandler? quitting = null;

        // Call this method at the end of your program to invoke all quitting handlers
        public static void InvokeQuitting()
        {
            quitting?.Invoke();
        }

        public static void Quit()
        {
            var handlers = quitting;
            if (handlers != null)
            {
                foreach (var handler in handlers.GetInvocationList())
                {
                    try
                    {
                        ((QuittingHandler)handler)();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"Exception in quitting handler: {ex.Message}");
                    }
                }
            }
        }
    }
}
