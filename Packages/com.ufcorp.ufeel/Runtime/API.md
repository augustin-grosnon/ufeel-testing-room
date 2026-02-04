# 📘 UFeelAPI – Unity Gameplay Integration Guide

## Overview

`UFeelAPI` is a **singleton Unity MonoBehaviour** that allows Unity games to react in real time to **player signals** such as:

* 🧠 **Emotions**
* 👁️ **Eye tracking (gaze direction)**
* 🎙️ **Speech (keywords / phrases)**
* ❤️ **Heart rate**

The API is designed to be **event-driven** and **gameplay-oriented**:
you define *rules* that automatically trigger actions when a condition becomes true.

No polling, no manual state checking every frame — the API handles it for you.

---

## 🚀 Getting Started

### Accessing the API

```csharp
UFeelAPI api = UFeelAPI.Instance;
```

`UFeelAPI` is a **persistent singleton**:

* Automatically created if not present in the scene
* Survives scene changes (`DontDestroyOnLoad`)

---

## 🔁 Core Concept: Rules

A **Rule** is composed of:

* a **condition** (emotion, direction, speech, heart rate…)
* an **action** (your gameplay code)
* a **mode**:

  * `Once` → triggered once, then removed
  * `Continuous` → triggered every frame while the condition is true

Internally, rules are evaluated **every frame in `Update()`**.

### RuleHandle

When you register a rule, you receive a `RuleHandle`:

```csharp
RuleHandle handle;
```

This handle allows you to **manually remove** the rule later if needed.

---

## 🧠 Emotion Detection

### Start / Stop

```csharp
api.StartEmotionDetection();
api.StopEmotionDetection();
```

Emotion detection **must be started** before accessing data or triggering rules.

---

### Reading Emotion Data

```csharp
EmotionData? emotions = api.GetCurrentEmotions();
EmotionData.EmotionType? dominant = api.GetDominantEmotion();
```

* Returns `null` if the system is not running
* `GetDominantEmotion()` returns the strongest detected emotion

---

### Triggering Gameplay from Emotions

#### Trigger Once

```csharp
api.TriggerActionOnEmotionOnce(
    EmotionData.EmotionType.Happy,
    () => Debug.Log("Player is happy!")
);
```

#### Trigger Continuously

```csharp
RuleHandle handle = api.TriggerActionOnEmotionContinuous(
    EmotionData.EmotionType.Angry,
    () => TakeDamageOverTime()
);
```

#### Removing a Rule

```csharp
api.RemoveRule(handle);
```

---

## 👁️ Eye Tracking (Gaze Direction)

### Start / Stop

```csharp
api.StartEyeTrackingDetection();
api.StopEyeTrackingDetection();
```

---

### Reading Gaze Direction

```csharp
EyeTrackingData? data = api.GetCurrentDirections();
EyeTrackingData.EyeTrackingType? direction = api.GetDominantDirection();
```

---

### Triggering Gameplay from Gaze

```csharp
api.TriggerActionOnDirectionOnce(
    EyeTrackingData.EyeTrackingType.Left,
    () => OpenLeftDoor()
);
```

or continuously:

```csharp
api.TriggerActionOnDirectionContinuous(
    EyeTrackingData.EyeTrackingType.Up,
    () => AimUpwards()
);
```

---

## 🎙️ Speech Detection

### Start / Stop

```csharp
api.StartSpeechDetection();
api.StopSpeechDetection();
```

---

### Reading Current Speech

```csharp
string spokenText = api.GetCurrentSpeech();
```

Returns `null` if speech detection is not running.

---

### Triggering Actions from Speech

Speech rules trigger when the **detected text is contained inside the target string**
(case-insensitive).

```csharp
api.TriggerActionOnSpeechOnce(
    "open the door",
    () => OpenDoor()
);
```

Continuous mode:

```csharp
api.TriggerActionOnSpeechContinuous(
    "attack",
    () => TriggerCombatMode()
);
```

---

## ❤️ Heart Rate Detection

### Start / Stop

```csharp
api.StartHeartRateDetection();
api.StopHeartRateDetection();
```

---

### Reading Heart Rate

```csharp
int? bpm = api.GetCurrentHeartRate();
```

---

### Triggering Actions from Heart Rate

You can define a **target BPM** with an optional **tolerance**.

```csharp
api.TriggerActionOnHeartRateOnce(
    rate: 120,
    action: () => EnterStressMode(),
    tolerance: 10
);
```

This triggers if the heart rate is between **110 and 130 BPM**.

Continuous variant:

```csharp
api.TriggerActionOnDirectionContinuous(
    rate: 90,
    action: () => CalmState(),
    tolerance: 5
);
```

---

## 🧹 Removing Rules Manually

Any rule can be removed at runtime:

```csharp
RuleHandle handle = api.TriggerActionOnEmotionContinuous(...);
api.RemoveRule(handle);
```

Useful for:

* state machines
* temporary gameplay effects
* cutscenes

---

## 🛑 Stopping Everything

```csharp
api.StopAPI();
```

This will:

* stop **all detections**
* notify the backend systems
* shut down the Python server

Automatically called when the GameObject is disabled.

---

## 📊 Debugging

```csharp
api.Status();
```

Logs the running state of all systems in the Unity Console.

---

## ⚠️ Important Notes & Limitations

* Detection systems **must be started explicitly**
* Rules are evaluated **every frame**
* Actions should be **fast** (avoid heavy logic inside rules)
* Speech matching uses `Contains` (not exact match)
* Backend communication is assumed to be active

---

## ✅ Typical Usage Pattern

```csharp
async void Start()
{
    UFeelAPI instance = UFeelAPI.Instance;
    Debug.Log("Hello UFEEL User");
    await Task.Delay(5000);

    instance.StartEmotionDetection();
    instance.Status();

    await Task.Delay(5000);

    Debug.Log("Here is the current emotion " + instance.GetCurrentEmotions());
    Debug.Log("Here is the dominant emotion " + instance.GetDominantEmotion());

    instance.TriggerActionOnEmotionOnce(EmotionData.EmotionType.Anger, async () =>
    {
        instance.StopEmotionDetection();
        instance.Status();
        instance.StartEyeTrackingDetection();
        instance.Status();

        await Task.Delay(5000);

        Debug.Log("Here is the current eye data " + instance.GetCurrentDirections());
        Debug.Log("Here is the dominant direction " + instance.GetDominantDirection());

        instance.TriggerActionOnDirectionOnce(EyeTrackingData.EyeTrackingType.UpRight, () =>
        {
            instance.StopEyeTrackingDetection();

            instance.StartSpeechDetection();

            // Continuous Emotion
            instance.StartEmotionDetection();
            UFeelAPI.RuleHandle rd = instance.TriggerActionOnEmotionContinuous(EmotionData.EmotionType.Happiness, async () =>
            {
                await Task.Delay(1000);
                Debug.Log("Emotion Continuellement");
            });
            //

            instance.Status();

            instance.TriggerActionOnSpeechOnce("Camion", async () =>
            {
                Debug.Log("Here is the current speech " + instance.GetCurrentSpeech());

                // Remove Continuous Emotion
                instance.RemoveRule(rd);
                instance.StopEmotionDetection();
                //

                instance.StopSpeechDetection();
                instance.StartHeartRateDetection();
                instance.Status();

                await Task.Delay(5000);

                instance.TriggerActionOnHeartRateOnce(80, () =>
                {
                    StopUnity(instance);
                });
            });
        });
    });
}
```