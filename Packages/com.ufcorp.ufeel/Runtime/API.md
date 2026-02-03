# 📘 UFeelAPI Documentation

## Overview

`UFeelAPI` is a **singleton Unity MonoBehaviour class** designed to integrate **emotion detection** and **eye tracking** systems into a Unity game. It interfaces with two underlying data receivers: `EmotionReceiver` and `EyeTrackingReceiver`, and allows enabling/disabling tracking, retrieving current data, and triggering actions based on dominant states.

---

## 🔧 Singleton Access

```csharp
public static UFeelAPI Instance
```

Access the global instance of `UFeelAPI`.

---

## 📊 Status

```csharp
public void status()
```

Prints the current operational status of the emotion and eye tracking systems to the Unity Console.

---

## 🧠 Emotion Detection

### Start Emotion Detection

```csharp
public void StartEmotionDetection()
```

Initializes and starts the emotion detection system.

### Stop Emotion Detection

```csharp
public void StopEmotionDetection()
```

Stops the emotion detection system and cleans up resources.

### Get Current Emotions

```csharp
public EmotionData? GetCurrentEmotions()
```

Returns the latest emotion data received.

### Get Dominant Emotion

```csharp
public EmotionData.EmotionType? GetDominantEmotion()
```

Returns the most dominant emotion detected at the moment.

### Trigger Action by Emotion

```csharp
public void TriggerActionIfEmotion(EmotionData.EmotionType emotion, Action action)
```

Invokes the provided `action` **only if** the currently dominant emotion matches the specified one.

### Enable/Disable Camera Feed for Emotion

```csharp
public static void enableCameraEmotion()
public static void disableCameraEmotion()
```

Enables or disables the emotion detection camera stream (stubbed function; implement sending logic if needed).

---

## 👁️ Eye Tracking Detection

### Start Eye Tracking

```csharp
public void StartEyeTrackingDetection()
```

Initializes and starts the eye tracking system.

### Stop Eye Tracking

```csharp
public void StopEyeTrackingDetection()
```

Stops the eye tracking system and cleans up resources.

### Get Current Eye Direction

```csharp
public EyeTrackingData? GetCurrentDirections()
```

Returns the latest eye tracking data received.

### Get Dominant Direction

```csharp
public EyeTrackingData.EyeTrackingType? GetDominantDirection()
```

Returns the most dominant gaze direction currently detected.

### Trigger Action by Eye Direction

```csharp
public void TriggerActionIfDirection(EyeTrackingData.EyeTrackingType direction, Action action)
```

Invokes the provided `action` **only if** the current dominant gaze direction matches the specified one.

### Enable/Disable Camera Feed for Eye Tracking

```csharp
public static void enableCameraEyeTracking()
public static void disableCameraEyeTracking()
```

Enables or disables the eye tracking camera stream (stubbed function; implement sending logic if needed).

---

## 🔌 Global Stop

```csharp
public void stopAPI()
```

Stops all tracking systems (emotion and eye tracking) and shuts down the Python backend server if used.

---

## 🧱 Internal Implementation Notes

* **`toggleCamera` and `toggleEmotionDetection` / `toggleEyeTrackingDetection`** are stubbed for now (do nothing).
* Actual data is fetched from:

  * `_emotionReceiver._currentEmotions`
  * `_eyeTrackingReceiver.CurrentEyeTrackingData`
* Receivers must be running to access live data. Call `StartEmotionDetection()` or `StartEyeTrackingDetection()` first.

---

## 🚨 Known Issues

* 🔧 Network communication for toggling features (`SendData`) is stubbed and needs implementation.

---

## ✅ Example Usage

```csharp
void Start()
{
    UFeelAPI.Instance.StartEmotionDetection();

    if (UFeelAPI.Instance.GetDominantEmotion() == EmotionData.EmotionType.Happy)
    {
        Debug.Log("Player is happy!");
    }

    UFeelAPI.Instance.StopEmotionDetection();

    UFeelAPI.Instance.StartEyeTrackingDetection();

    UFeelAPI.Instance.TriggerActionIfDirection(
        EyeTrackingData.EyeTrackingType.Left,
        () => Debug.Log("Player is looking left.")
    );

    UFeelAPI.Instance.stopAPI();
}
```