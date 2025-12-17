# UFeel – Architecture Overview

This document describes the **complete architecture** of the UFeel project, including  
Unity structure, Python backend, plugin submodules, scenes, and communication flow.

---

# 1. High-Level Architecture

1. **Capture**  
   Collecting Webcam, microphone and biometric sensors raw inputs (video, audio, heart rate).

2. **Inference**  
   A Python server processes these inputs with models (emotion, eye-tracking, speech-to-text, heartbeat).

3. **Transmission**  
   Processed data is sent to Unity through a socket connection (JSON messages).

4. **Reception**  
   Unity receives and updates data via:  
   - `EmotionReceiver`  
   - `EyeTrackingReceiver`  
   - `SpeechReceiver`  
   - `HeartbeatSingleton`

5. **API Layer**  
   All data is centralized into the `UfeelAPI` singleton.

6. **Gameplay Integration**  
   Testing Room scenes and developer scripts read `UfeelAPI` to drive gameplay interactions.

7. **Future Direction**  
   Transition from Python backend to compiled native libraries and engine plugins (Unity/Unreal/Godot).

---

# 2. Submodules Overview

### 🎯 Unity Submodules

| Submodule | Description |
|-----------|-------------|
| **UfeelAPI** | Public API for developers (emotion, eye tracking, speech, heartbeat). |
| **EmotionReceiver** | Receives emotion data from Python. |
| **EyeTrackingReceiver** | Receives gaze direction from Python. |
| **SpeechReceiver** | Receives speech-to-text data. |
| **HeartBeatReceiver** | Receives biometric sensor data |
| **TestingRoom** | Demo scenes showcasing plugin capabilities. |

---

# 3. Unity Project Folder Structure

Below are the main folders included in the UFeel Unity package and their purpose:

### **UFeel/**
Contains all core components of the plugin (scripts, scenes, UI, prefabs, etc.).

### UFeel/Assets**
Contains all 

### UFeel/Documentation**
Contains all documentation of the project

### **UFeel/Packages/**
Contains data receivers, utilities, and demo controllers for testing room scenes.

### **UFeel/PythonServers/**
Handles all real-time processing for emotion recognition, eye-tracking, speech-to-text, and biometrics.  
It runs the machine-learning models, reads webcam/microphone data, and sends the processed results to Unity through a socket connection.

---

# 4. Python Server Architecture

### Components
- `emotion_server.py`
- `eye_tracking_server.py`
- `speech_server.py`
- (add biometric sensor)
- Shared socket server core
- Model inference utilities

### Responsibilities
- Listen to webcam and voice stream 
- Run Machine Learning models (emotion, gaze, speech, biometric sensor)  
- Send outputs to Unity

---

# 5. Communication Layer

Unity ↔ Python communication uses:

- UDP sockets  
- JSON messages  
- Async listeners on the Unity side  
- Continuous streaming from Python models  

---

# 6. Code Zones

- **Interaction Layer**: Testing Room scripts  
- **API Layer**: UfeelAPI  
- **Data Layer**: Receivers + Data Structures  
- **Networking Layer**: Socket managers  
- **Inference Layer**: Python Machine Learning models  
