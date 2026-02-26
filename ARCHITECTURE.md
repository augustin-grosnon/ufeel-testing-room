# Architecture

> [<- Back to README](README.md)

UFeel Testing Room is a Unity project demonstrating the **UFeel** biometric package. Unity spawns a Python server that processes webcam/sensor data and streams results back via TCP.

## Overview

```mermaid
graph TD
    subgraph Unity["Unity (C#)"]
        Launcher[LauncherScript] --> API[UFeelAPI]
        API --> Rules[Rules Engine]
        Rules --> Game[Game Logic / Scenes]

        API --> ER[EmotionReceiver :4100]
        API --> ET[EyeTrackingReceiver :4000]
        API --> ST[SpeechToTextReceiver :3900]
        API --> HR[HeartRateSensorReceiver :3800]

        API --> PSC[PythonServerController]
    end

    subgraph Python["Python Server (subprocess)"]
        Main[main.py] --> DP[DataProcessor]
        DP --> ED[EmotionDetector]
        DP --> EY[EyeTracker]
        DP --> STT[SpeechToText]
        DP --> HRS[HeartRateSensor]
    end

    PSC -->|spawn process| Main
    ED -->|TCP :4100| ER
    EY -->|TCP :4000| ET
    STT -->|TCP :3900| ST
    HRS -->|TCP :3800| HR
    DP -->|OpenCV webcam| Camera[(Webcam)]
```

## Key Components

### UFeel Package (`Packages/com.ufcorp.ufeel`)

| Component | Role |
|---|---|
| `UFeelAPI` | Singleton entry point. Starts/stops detectors, exposes data getters, manages rules. |
| `ClientBase` | Abstract TCP listener. Each receiver extends it to decode JSON messages on a dedicated thread. |
| `PythonServerController` | Spawns/kills the Python process from Unity's lifecycle. |
| `Rules Engine` | Condition->Action pairs evaluated every `Update()`. Supports one-shot and continuous rules. |

### Python Server (`PythonServer/`)

| Module | Role |
|---|---|
| `DataProcessor` | Main loop - reads webcam frames, dispatches to each detector. |
| `EmotionDetector` | MediaPipe-based facial emotion detection. |
| `EyeTracker` | Gaze direction estimation. |
| `SpeechToText` | Microphone transcription. |
| `HeartRateSensor` | rPPG heart-rate estimation from camera. |

### Testing Room (`Assets/Scripts/TestingRoom`)

First-person experience used to validate the UFeel package: room navigation, carousel, doors, and scene transitions driven by UFeel rules.

## Data Flow

```mermaid
sequenceDiagram
    participant Game
    participant UFeelAPI
    participant Receiver
    participant Python

    Game->>UFeelAPI: StartAPI()
    UFeelAPI->>Python: spawn subprocess
    Game->>UFeelAPI: StartEmotionDetection()
    UFeelAPI->>Receiver: begin listening :4100
    Python-->>Receiver: JSON over TCP (continuous)
    UFeelAPI->>Game: rules fire -> callbacks
```

**See also:** [API Reference](Packages/com.ufcorp.ufeel/Runtime/API.md) - [Setup Instructions](Documentation/TestingRoom/SetupInstructions.md)
