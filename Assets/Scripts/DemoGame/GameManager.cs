using System.Collections;
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
    [SerializeField] private Transform _player;

    private string _currentTarget;
    private float _matchTimer = 0f;
    private bool _successTriggered = false;

    void Start()
    {
        var _ = EmotionReceiver.Instance;

        SetNextTargetEmotion();
    }

    void Update()
    {
        if (_successTriggered)
            return;

        string detectedEmotion = DetermineDominantEmotion(EmotionReceiver.CurrentEmotions);

        if (detectedEmotion == _currentTarget)
        {
            _matchTimer += Time.deltaTime;

            if (_matchTimer < _requiredMatchDuration)
            {
                _instructionText.color = new Color(1f, 0.65f, 0f);
            }
            else
            {
                _instructionText.color = Color.green;
                _successTriggered = true;
                _doorController.ToggleDoor();

                StartCoroutine(WaitAfterSuccess());
            }
        }
        else
        {
            _matchTimer = 0f;
            _instructionText.color = Color.red;
        }
    }

    private IEnumerator WaitAfterSuccess()
    {
        yield return new WaitForSeconds(3f);

        _doorController.ToggleDoor();

        if (_player != null)
        {
            _player.position = new Vector3(0f, 1f, -0.7f);
        }

        SetNextTargetEmotion();
        _matchTimer = 0f;
        _successTriggered = false;
    }

    private void SetNextTargetEmotion()
    {
        _currentTarget = _targetEmotions[Random.Range(0, _targetEmotions.Count)];
        _instructionText.text = "Show " + _currentTarget;
        _instructionText.color = Color.red;
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
