using UnityEngine;
using UFeel;
using System.Threading.Tasks;
using System.Diagnostics;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LauncherScript : MonoBehaviour
{
    async void Start()
    {
        await UFeelAPI.StartAPI().ConfigureAwait(true);

        UFeelAPI.StartEmotionDetection();
        UFeelAPI.Status();


        Stopwatch stopwatch = Stopwatch.StartNew();

        EmotionData? currentEmotions = await UFeelAPI.GetCurrentEmotionsData()
            .ConfigureAwait(true);

        stopwatch.Stop();

        UnityEngine.Debug.Log($"GetCurrentEmotionsData took {stopwatch.ElapsedMilliseconds} ms");
        UnityEngine.Debug.Log($"Result {currentEmotions}");
    }

    void Update()
    {
        return;
    }
}
