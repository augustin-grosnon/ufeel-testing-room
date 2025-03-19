using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private readonly List<string> _targetEmotions = new()
    {
        "happy",
        "surprised",
        "sad",
        "angry",
        "scared",
    };

    private readonly Dictionary<string, float> _emotionThresholds = new()
    {
        { "happy", 0.2f },
        { "surprised", 0.1f },
        { "sad", 0.2f },
        { "angry", 0.05f },
        { "scared", 0.0f },
    };

    [SerializeField] private Text _instructionText;
    [SerializeField] private DoorController _doorController;
    [SerializeField] private float _requiredMatchDuration = 1f;
    [SerializeField] private Transform _player;

    private string _currentTarget;
    private string _previousTarget = "";
    private float _matchTimer = 0f;
    private bool _successTriggered = false;
    private readonly KeyCode[] _devCode = { KeyCode.U, KeyCode.F };
    private int _devCodeProgress = 0;
    private float _devCodeTimeout = 2f;
    private float _devCodeTimer = 0f;

    void Start()
    {
        var _ = EmotionReceiver.Instance;

        SetNextTargetEmotion();
    }

    void Update()
    {
        if (_successTriggered)
            return;

        HandleDebugSkipInput();

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

    private void HandleDebugSkipInput()
    {
        if (_devCodeProgress > 0)
        {
            _devCodeTimer += Time.deltaTime;

            if (_devCodeTimer > _devCodeTimeout)
            {
                _devCodeProgress = 0;
                _devCodeTimer = 0f;
            }
        }

        if (Input.GetKeyDown(_devCode[_devCodeProgress]))
        {
            _devCodeProgress++;
            _devCodeTimer = 0f;

            if (_devCodeProgress >= _devCode.Length)
            {
                Debug.Log("Debug skip triggered!");
                _devCodeProgress = 0;
                _devCodeTimer = 0f;

                _matchTimer = 0f;
                _successTriggered = false;
                SetNextTargetEmotion();
            }
        }
    }

    private IEnumerator WaitAfterSuccess()
    {
        yield return new WaitForSeconds(3f);

        _doorController.ToggleDoor();

        yield return new WaitForSeconds(1f);

        if (_player != null)
        {
            Rigidbody playerRigidbody = _player.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                playerRigidbody.MovePosition(new Vector3(0f, 1f, -2f));
            }
            else
            {
                _player.position = new Vector3(0f, 1f, -2f);
            }
        }

        SetNextTargetEmotion();
        _matchTimer = 0f;
        _successTriggered = false;
    }

    private void SetNextTargetEmotion()
    {
        string newTarget;

        do
        {
            newTarget = _targetEmotions[Random.Range(0, _targetEmotions.Count)];
        }
        while (newTarget == _currentTarget);

        _previousTarget = _currentTarget;
        _currentTarget = newTarget;

        _instructionText.text = "Be " + _currentTarget;
        _instructionText.color = Color.red;
    }

    private string DetermineDominantEmotion(EmotionData data)
    {
        Dictionary<string, float> values = new()
        {
            { "happy", data.happiness },
            { "surprised", data.surprise },
            { "sad", data.sadness },
            { "angry", data.anger },
            { "scared", data.fear },
        };

        var filteredValues = values
            .Where(kv => kv.Value >= _emotionThresholds[kv.Key])
            .ToDictionary(kv => kv.Key, kv => kv.Value);

        if (filteredValues.Count == 0) return "";

        return filteredValues.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
    }
}

// TODO: add score nombre porte ouverte
// TODO: poser une pastèque dans la room à chaque porte ouverte
