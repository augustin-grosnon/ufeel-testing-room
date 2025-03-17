using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private readonly List<string> _targetEmotions = new()
    {
        "Happiness",
        "Surprise",
        // "Sadness",
        "Anger",
        // "Fear",
    };

    [SerializeField] private Text _instructionText;
    [SerializeField] private DoorController _doorController;
    [SerializeField] private float _requiredMatchDuration = 1f;

    private string _currentTarget;
    private float _matchTimer = 0f;

    void Start()
    {
        var _ = EmotionReceiver.Instance;

        SetNextTargetEmotion();
    }

    void Update()
    {
        string detectedEmotion = DetermineDominantEmotion(EmotionReceiver.CurrentEmotions);

        if (detectedEmotion == _currentTarget)
        {
            _matchTimer += Time.deltaTime;
            if (_matchTimer >= _requiredMatchDuration)
            {
                _doorController.OpenDoor();
                Invoke(nameof(SetNextTargetEmotion), 2f);
                _matchTimer = 0f;
            }
        }
        else
        {
            _matchTimer = 0f;
        }
    }

    private void SetNextTargetEmotion()
    {
        _currentTarget = _targetEmotions[Random.Range(0, _targetEmotions.Count)];
        _instructionText.text = "Show " + _currentTarget;
        _doorController.CloseDoor();
    }

    private string DetermineDominantEmotion(EmotionData data)
    {
        Dictionary<string, float> values = new()
        {
            { "Happiness", data.happiness },
            { "Surprise", data.surprise },
            // { "Sadness", data.sadness },
            { "Anger", data.anger },
            // { "Fear", data.fear },
        };

        return values.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
    }
}
