using System;
using System.Collections.Generic;
using UnityEngine;

public class UFeelDebugHUD : MonoBehaviour
{
    private static UFeelDebugHUD _instance;

    internal static float MaxWidth = 1000;
    internal static float BaseX = 10;
    internal static float BaseY = 10;
    internal static float LineHeight = 20;
    internal static float Padding = 10;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoCreate()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (_instance != null)
            return;

        GameObject obj = new GameObject("UFeelDebugHUD");
        _instance = obj.AddComponent<UFeelDebugHUD>();
        DontDestroyOnLoad(obj);
#endif
    }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    internal const bool DEBUG_MODE = true;
#else
    internal const bool DEBUG_MODE = false;
#endif

    private static readonly Dictionary<string, Func<string>> _entries = new();

    public static void Set(string label, Func<string> getter)
    {
        _entries[label] = getter;
    }

    public static void Remove(string label)
    {
        _entries.Remove(label);
    }

    public static void Clear()
    {
        _entries.Clear();
    }

    private static string TruncateLine(string text)
    {
        if (GUI.skin.label.CalcSize(new GUIContent(text)).x <= MaxWidth)
            return text;

        const string suffix = "...";

        if (GUI.skin.label.CalcSize(new GUIContent(suffix)).x > MaxWidth)
            return suffix;

        int left = 0;
        int right = text.Length;

        while (left < right)
        {
            int mid = (left + right + 1) / 2;

            string candidate = text[..mid] + suffix;
            float w = GUI.skin.label.CalcSize(new GUIContent(candidate)).x;

            if (w <= MaxWidth)
                left = mid;
            else
                right = mid - 1;
        }

        return text[..left] + suffix;
    }



    private static (float width, List<string> lines) GetWidthAndLines()
    {
        List<string> lines = new();

        float maxLineWidth = 0f;

        foreach (var kvp in _entries)
        {
            string label = kvp.Key;
            string value = "Unknown";

            try
            {
                value = kvp.Value?.Invoke() ?? "Unknown";
            }
            catch
            {
                value = "Error";
            }

            string line = $"{label}: {value}";
            line = TruncateLine(line);
            lines.Add(line);

            Vector2 size = GUI.skin.label.CalcSize(new GUIContent(line));
            if (size.x > maxLineWidth)
                maxLineWidth = size.x;
        }

        float width = maxLineWidth + Padding * 2 + 10;
        return (width, lines);
    }


    void OnGUI()
    {
        if (!DEBUG_MODE)
            return;

        var (width, lines) = GetWidthAndLines();
        float height = 40 + _entries.Count * LineHeight;

        GUI.Box(new Rect(BaseX, BaseY, width, height), "UFeel Debug HUD");

        float currentY = BaseY + 25;
        foreach (var line in lines)
        {
            GUI.Label(new Rect(BaseX + Padding, currentY, width - Padding * 2, LineHeight), line);
            currentY += LineHeight;
        }
    }
}
