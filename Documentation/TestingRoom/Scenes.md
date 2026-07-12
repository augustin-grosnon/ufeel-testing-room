# Scene Reference

> [<- Testing Room](README.md)

## Global Controls

These shortcuts are available in all scenes:

| Key                           | Action                                     |
| ----------------------------- | ------------------------------------------ |
| **P**                         | Permanently stop the Python camera process |
| **,** (M on QWERTY keyboards) | Open the debug/menu interface              |

---

# Base Scene (TestingRoom)

## Role

Main hub used to access all available experiences.

## Features

* First-person controller.
* Mouse controls camera rotation.
* **W**: Move forward.
* **S**: Move backward.
* Scene selection through interactive doors.
* Lighting is intentionally disabled.

All other scenes can be accessed by entering the corresponding door.

---

# Emotion-Based Experiences

## Maze

**Role:** Emotion-driven labyrinth experience.

**Main Script:** `MazeManager`

**See also:** [Maze README](../Emotions/Maze/README.md)

---

## Animals

**Role:** Emotion-based memory game.

**Main Script:** `AnimalsManager`

**See also:** [Animals README](../Emotions/Animals/README.md)

---

# Eye Tracking Experiences

## Car

**Role:** Vehicle control using eye tracking.

**Main Script:** `VehicleController`

**See also:** [Eye Tracking README](../EyeTracking/Car/README.md)

---

## Survivor

**Role:** Eye-tracking survival experience.

**Status:** Planned for future development.

---

# Speech-To-Text Experiences

## EscapeRoom

**Role:** Voice-controlled escape room.

**Main Script:** `SpeechManager`

**See also:** [EscapeRoom README](../SpeechToText/EscapeRoom/README.md)

---

# Heart Rate Experiences

## RhythmGame

**Role:** Heart-rate-adaptive rhythm game.

**Main Script:** `GameManager`

**See also:** [RhythmGame README](../HeartRateSensor/RhythmGame/README.md)
