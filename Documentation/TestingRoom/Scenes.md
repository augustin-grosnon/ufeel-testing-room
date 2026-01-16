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
