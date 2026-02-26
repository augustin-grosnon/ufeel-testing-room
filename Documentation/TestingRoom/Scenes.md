# Scene Reference

> [<- Testing Room](README.md)

## Base Scene (TestingRoom)

- **Role**: Hub for scene transitions.
- Carousel for selecting scenes, doors using `DoorController`.
- Lighting disabled intentionally.

## Eye Tracking Scene (TestingRoom_EyeTracking)

- **Role**: Player steers a vehicle using gaze direction.
- Left/right gaze -> steering; up/down speed control (coming soon).
- Key script: `VehicleController`

## Emotion Scene (TestingRoom_EmotionDetection)

- **Role**: Emotion recognition controls door access.
- Key script: `EmotionGameManager`

**See also:** [Setup Instructions](SetupInstructions.md) - [Testing & Debugging](TestingTips.md)
