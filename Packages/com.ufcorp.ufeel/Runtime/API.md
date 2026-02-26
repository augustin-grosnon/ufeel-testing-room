# UFeelAPI – Unity API Reference

> [<- Back to README](../../../README.md)

`UFeelAPI` is a static singleton MonoBehaviour for reacting to player biometric signals in real time:

* Emotions
* Eye tracking (gaze direction)
* Speech (keywords / phrases)
* Heart rate

Define *rules* - condition + action pairs - that fire automatically rather than polling each frame.

## Getting Started

All methods are static. Required objects are created automatically if absent from the scene and persist across scene loads (`DontDestroyOnLoad`).

## Core Concept: Rules

A rule has a **condition**, an **action**, and a **mode**:

* `Once` - triggered once, then removed
* `Continuous` - triggered every frame while the condition holds

Rules are evaluated every `Update()`.

### RuleKey

When you register a rule, you receive a `RuleKey`:

```csharp
RuleKey key;
```

This key allows you to **manually remove** the rule later if needed.

## Emotion Detection

### Start / Stop

```csharp
UFeelAPI.StartEmotionDetection();
UFeelAPI.StopEmotionDetection();
```

Emotion detection must be started before reading data or registering rules.

### Reading Emotion Data

```csharp
EmotionData? emotions = UFeelAPI.GetCurrentEmotionsData();
EmotionData.EmotionType? dominant = UFeelAPI.GetDominantEmotion();
```

Returns `null` if the system is not running. `GetDominantEmotion()` returns the highest-confidence emotion.

### Triggering Rules from Emotions

#### Trigger Once

```csharp
UFeelAPI.TriggerActionOnEmotionOnce(
    EmotionData.EmotionType.Happy,
    () => Debug.Log("Player is happy!")
);
```

#### Trigger Continuously

```csharp
RuleKey key = UFeelAPI.TriggerActionOnEmotionContinuous(
    EmotionData.EmotionType.Angry,
    () => TakeDamageOverTime()
);
```

#### Removing a Rule

```csharp
UFeelAPI.RemoveRule(key);
```

## Eye Tracking

### Start / Stop

```csharp
UFeelAPI.StartEyeTrackingDetection();
UFeelAPI.StopEyeTrackingDetection();
```

### Reading Gaze Direction

```csharp
EyeTrackingData? data = UFeelAPI.GetCurrentDirections();
EyeTrackingData.EyeTrackingType? direction = UFeelAPI.GetDominantDirection();
```

### Triggering Gameplay from Gaze

```csharp
UFeelAPI.TriggerActionOnDirectionOnce(
    EyeTrackingData.EyeTrackingType.Left,
    () => OpenLeftDoor()
);
```

or continuously:

```csharp
UFeelAPI.TriggerActionOnDirectionContinuous(
    EyeTrackingData.EyeTrackingType.Up,
    () => AimUpwards()
);
```

## Speech Detection

### Start / Stop

```csharp
UFeelAPI.StartSpeechDetection();
UFeelAPI.StopSpeechDetection();
```

### Reading Current Speech

```csharp
string spokenText = UFeelAPI.GetCurrentSpeech();
```

Returns `null` if speech detection is not running.

### Triggering Rules from Speech

Triggers when the detected text **contains** the target string (case-insensitive).

```csharp
UFeelAPI.TriggerActionOnSpeechOnce(
    "open the door",
    () => OpenDoor()
);
```

Continuous mode:

```csharp
UFeelAPI.TriggerActionOnSpeechContinuous(
    "attack",
    () => TriggerCombatMode()
);
```

## Heart Rate Detection

### Start / Stop

```csharp
UFeelAPI.StartHeartRateDetection();
UFeelAPI.StopHeartRateDetection();
```

### Reading Heart Rate

```csharp
int? bpm = UFeelAPI.GetCurrentHeartRate();
```

### Triggering Rules from Heart Rate

Define a target BPM with an optional tolerance (±).

```csharp
UFeelAPI.TriggerActionOnHeartRateOnce(
    rate: 120,
    action: () => EnterStressMode(),
    tolerance: 10
);
```

Triggers if BPM is within ±tolerance of the target (here: 110–130).

Continuous variant:

```csharp
UFeelAPI.TriggerActionOnDirectionContinuous(
    rate: 90,
    action: () => CalmState(),
    tolerance: 5
);
```

## Removing Rules

```csharp
RuleKey key = UFeelAPI.TriggerActionOnEmotionContinuous(...);
UFeelAPI.RemoveRule(key);
```

## Stopping the API

```csharp
UFeelAPI.StopAPI();
```

Stops all detectors and shuts down the Python server. Called automatically when the `UFeelAPI` GameObject is disabled.

## Debugging

```csharp
UFeelAPI.Status();
```

Logs the running state of all systems in the Unity Console.

## Limitations

* Each detector must be started explicitly.
* Rule actions run every frame - keep them fast.
* Speech matching uses `Contains`, not exact match.
* The Python server must be running for data to arrive.

## Typical Usage Pattern

```csharp
async void Start()
{
    await UFeelAPI.StartAPI();

    UFeelAPI.StartEmotionDetection();
    UFeelAPI.Status();

    Debug.Log("Here is the current emotion " + UFeelAPI.GetCurrentEmotionsData());
    Debug.Log("Here is the dominant emotion " + UFeelAPI.GetDominantEmotion());

    UFeelAPI.TriggerActionOnEmotionOnce(EmotionData.EmotionType.Anger, async () =>
    {
        UFeelAPI.StopEmotionDetection();
        UFeelAPI.Status();
        UFeelAPI.StartEyeTrackingDetection();
        UFeelAPI.Status();

        Debug.Log("Here is the current eye data " + UFeelAPI.GetCurrentDirections());
        Debug.Log("Here is the dominant direction " + UFeelAPI.GetDominantDirection());

        UFeelAPI.TriggerActionOnDirectionOnce(EyeTrackingData.EyeTrackingType.UpRight, () =>
        {
            UFeelAPI.StopEyeTrackingDetection();

            UFeelAPI.StartSpeechDetection();

            // Continuous Emotion
            UFeelAPI.StartEmotionDetection();
            RuleKey key = UFeelAPI.TriggerActionOnEmotionContinuous(EmotionData.EmotionType.Happiness, async () =>
            {
                await Task.Delay(1000);
                Debug.Log("Emotion Continuellement");
            });
            //

            UFeelAPI.Status();

            UFeelAPI.TriggerActionOnSpeechOnce("Camion", async () =>
            {
                Debug.Log("Here is the current speech " + UFeelAPI.GetCurrentSpeech());

                // Remove Continuous Emotion
                UFeelAPI.RemoveRule(key);
                UFeelAPI.StopEmotionDetection();
                //

                UFeelAPI.StopSpeechDetection();
                UFeelAPI.StartHeartRateDetection();
                UFeelAPI.Status();

                UFeelAPI.TriggerActionOnHeartRateOnce(80, () =>
                {
                    UFeelAPI.StopAPI();
                });
            });
        });
    });
}
```

**See also:** [<- README](../../../README.md) - [Architecture](../../../ARCHITECTURE.md) - [Testing & Debugging](../../../Documentation/TestingRoom/TestingTips.md)
