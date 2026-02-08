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

All the method of the Class `UFeelAPI` are static:

* Useful variable are automatically created if not present in the scene
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

### RuleKey

When you register a rule, you receive a `RuleKey`:

```csharp
RuleKey key;
```

This key allows you to **manually remove** the rule later if needed.

---

## 🧠 Emotion Detection

### Start / Stop

```csharp
UFeelAPI.StartEmotionDetection();
UFeelAPI.StopEmotionDetection();
```

Emotion detection **must be started** before accessing data or triggering rules.

---

### Reading Emotion Data

```csharp
EmotionData? emotions = UFeelAPI.GetCurrentEmotionsData();
EmotionData.EmotionType? dominant = UFeelAPI.GetDominantEmotion();
```

* Returns `null` if the system is not running
* `GetDominantEmotion()` returns the strongest detected emotion

---

### Triggering Gameplay from Emotions

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

---

## 👁️ Eye Tracking (Gaze Direction)

### Start / Stop

```csharp
UFeelAPI.StartEyeTrackingDetection();
UFeelAPI.StopEyeTrackingDetection();
```

---

### Reading Gaze Direction

```csharp
EyeTrackingData? data = UFeelAPI.GetCurrentDirections();
EyeTrackingData.EyeTrackingType? direction = UFeelAPI.GetDominantDirection();
```

---

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

---

## 🎙️ Speech Detection

### Start / Stop

```csharp
UFeelAPI.StartSpeechDetection();
UFeelAPI.StopSpeechDetection();
```

---

### Reading Current Speech

```csharp
string spokenText = UFeelAPI.GetCurrentSpeech();
```

Returns `null` if speech detection is not running.

---

### Triggering Actions from Speech

Speech rules trigger when the **detected text is contained inside the target string**
(case-insensitive).

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

---

## ❤️ Heart Rate Detection

### Start / Stop

```csharp
UFeelAPI.StartHeartRateDetection();
UFeelAPI.StopHeartRateDetection();
```

---

### Reading Heart Rate

```csharp
int? bpm = UFeelAPI.GetCurrentHeartRate();
```

---

### Triggering Actions from Heart Rate

You can define a **target BPM** with an optional **tolerance**.

```csharp
UFeelAPI.TriggerActionOnHeartRateOnce(
    rate: 120,
    action: () => EnterStressMode(),
    tolerance: 10
);
```

This triggers if the heart rate is between **110 and 130 BPM**.

Continuous variant:

```csharp
UFeelAPI.TriggerActionOnDirectionContinuous(
    rate: 90,
    action: () => CalmState(),
    tolerance: 5
);
```

---

## 🧹 Removing Rules Manually

Any rule can be removed at runtime:

```csharp
RuleKey key = UFeelAPI.TriggerActionOnEmotionContinuous(...);
UFeelAPI.RemoveRule(key);
```

Useful for:

* state machines
* temporary gameplay effects
* cutscenes

---

## 🛑 Stopping Everything

```csharp
UFeelAPI.StopAPI();
```

This will:

* stop **all detections**
* notify the backend systems
* shut down the Python server

Automatically called when the GameObject is disabled.

---

## 📊 Debugging

```csharp
UFeelAPI.Status();
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