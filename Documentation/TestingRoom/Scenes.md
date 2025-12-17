# Scene Architecture

## Base Scene (TestingRoom)

- **Purpose**: Acts as a hub for transitions.
- **Key Elements**:
  - Carousel for selecting interaction scenes.
  - Doors using `DoorController`.
- **Lighting**: None (disabled for stylistic purposes).

## Eye Tracking Scene (TestingRoom_EyeTracking)

- **Purpose**: Player drives a vehicle using gaze direction.
- **Input**: Eye direction (up/down for speed - coming soon, left/right for steering).
- **Important Scripts**:
  - `VehicleController`

## Emotion Scene (TestingRoom_EmotionDetection)

- **Purpose**: Gaze and emotion detection through facial expressions.
- **Logic**: Emotion recognition controls access through doors.
- **Important Scripts**:
  - `EmotionGameManager`

## Speech To Text Scene (TestingRoom_SpeechToText)

- **Purpose**: Voice‑based interactions allowing the player to open/close a door and switch on/off room lighting.
- **Input**: Spoken commands processed by speech‑to‑text ("open door", "close door", "lights on", "lights off").
- **Important Scripts**:
  - `SpeechManager`
  - `VoiceDoorController`

## Biometrics Sensors (TestingRoom_HeartBeat)
- **Purpose**: Displays player’s real-time heart rate in Unity as a demonstration of biometric sensor integration.
- **Input**: Heartbeat data streamed from an external biometric sensor.
- **Important Scripts**:
  - `HeartbeatSingleton`
