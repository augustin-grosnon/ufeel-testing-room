using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CubeColorChanger : MonoBehaviour
{
    private readonly Dictionary<string, Color> _emotionColors = new()
    {
        { "Happiness", Color.yellow },
        { "Surprise", Color.cyan },
        { "Sadness", Color.blue },
        { "Anger", Color.red },
        { "Neutral", Color.white },
        { "Fear", Color.magenta },
    };

    [Header("Settings")]
    [SerializeField] private float transitionSpeed = 3f;
    [SerializeField] private float emotionHistoryDuration = 1f;

    private Renderer _renderer;
    private Color _currentColor = Color.black;

    private struct TimedColor
    {
        public Color color;
        public float timestamp;
    }

    private readonly List<TimedColor> _colorHistory = new();

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _renderer.material.color = _currentColor;
    }

    void Update()
    {
        float currentTime = Time.time;
        _colorHistory.RemoveAll(c => currentTime - c.timestamp > emotionHistoryDuration);

        if (_colorHistory.Count > 0)
        {
            Color averageColor = AverageColors(_colorHistory.Select(c => c.color));
            _currentColor = Color.Lerp(_currentColor, averageColor, Time.deltaTime * transitionSpeed);
        }

        _renderer.material.color = _currentColor;
    }

    public void SetColor(string emotion)
    {
        if (emotion == "Neutral") { return; }
        if (_emotionColors.TryGetValue(emotion, out Color newColor))
        {
            _colorHistory.Add(new TimedColor { color = newColor, timestamp = Time.time });
            Debug.Log($"Received emotion: {emotion} → Color: {newColor}");
        }
        else
        {
            Debug.LogWarning($"Unrecognized emotion: {emotion}. Ignoring input.");
        }
    }

    private Color AverageColors(IEnumerable<Color> colors)
    {
        float r = 0f, g = 0f, b = 0f, a = 0f;
        int count = 0;

        foreach (var color in colors)
        {
            r += color.r;
            g += color.g;
            b += color.b;
            a += color.a;
            count++;
        }

        if (count == 0)
            return Color.black;

        return new Color(r / count, g / count, b / count, a / count);
    }
}
