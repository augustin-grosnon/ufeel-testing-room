# Changelog

All notable changes to this project are documented in this file.

The format follows semantic grouping (Added, Changed, Fixed, Merged, Documentation) depending on what was done during the month.

## [2026-02]

### Added

* New carousel system with reordered file structure
* `UFeelDebugHUG` debug class
* Debug mode for visual information
* Updated README for API
* Expanded and clarified `DISCLAIMER.md`, including prohibited use cases and non-responsibility clauses
* `CHANGELOG.md` updated based on the commits

### Changed

* Refactored `UFeelAPI` into static class
* Updated trigger behavior and Python server
* Merged API Unity into Testing Room
* Improved door selection GUI width

### Fixed

* Naming consistency for UFeel and asmdef files
* Unique file handling per data type
* Removed outdated code
* Fixed carousel doors behavior
* Fixed Testing Room scene doors
* Graphic settings adjustments

### Merged

* Testing Room into `main` branch

## [2026-01]

### Added

* Apache 2.0 license
* `CONTRIBUTING` details
* `DISCLAIMER.md`
* Documentation restructuring

## [2025-12]

### Added

* Speech-to-text detection in API
* Frame text rendering for debugging
* Heart rate sensor integration
* Heart rate receiver in Unity API

### Fixed

* Emotion detection in Unity
* Eye tracking integration in Unity

## [2025-10 – 2025-11]

### Added

* Doors for speech recognition and heartbeat
* Heartbeat and speech recognition scenes in Testing Room
* Base version without detection

### Changed

* Upgraded Unity to stable LTS version
* Restored flashlight support

## [2025-07]

### Changed

* Model activation logic improvements
* Documentation comments regarding multi-model frame handling

### Fixed

* Asynchronous Python process handling

## [2025-06]

### Added

* Base API
* API documentation
* TCP support and new `ClientBase` class
* Enable/disable Python process from C# server
* Integrated emotion detection into Testing Room
* Eye tracking finish line
* Eye tracking scene improvements (sphere movement, distance handling, vehicle control)
* Textured terrain for eye tracking scene
* Disable room after entering new scene

### Changed

* Door behavior improvements (lowering logic, rotation handling)
* Scene loading synchronization before door opening

## [2025-05]

### Added

* Testing Room base lighting and improvements
* Base carousel system
* Door prefab and automated door placement script
* Moving and lowering doors
* Door selection panel
* Chain extension system
* Scene swapping
* Door opening before scene swap
* Carousel builder with special doors
* Door selection by name
* Base corridor with additive scene loading
* Additive scene loading on door trigger

### Changed

* Directory restructuring
* Improved door prefab and setup workflow
* Larger door trigger
* Scene alignment adjustments
* Front wall split for transition logic
* Door positioning and wall placement
* Open wall during scene transition

## [2025-03]

### Added

* Initial project setup
* Unity project configuration
* Assembly definition and auto-reference script
* Base POC with canvas and cube
* Base scenes for Emotion Detection and Eye Tracking
* Emotion detection server and Unity integration
* Eye tracking server with automatic startup
* Generic receiver and server controller
* Singleton-based management for emotion and eye tracking
* Working base emotion game (happy, surprised, angry)
* Demo game scene with player
* Emotion-based door logic
* Score display
* Debug skip logic
* Emotion color handling
* Mediapipe face detection integration
* Smooth teleportation
* New eye tracker integration
* Multiple server handling with auto movement via eye tracking

### Changed

* Dependency updates (Barracuda, Unity version)
* Requirements updates
* Code cleanup and controller documentation
* Larger video processor window
* Demo scene refinements

### Fixed

* Removed unused Unity controllers
* Eye tracker usability improvements
* Removed unnecessary GitHub Actions job
* Disabled mirroring GitHub Actions on main repository

## [Project Initialization – 2025-03-06]

### Added

* Initial commit
* Embedded package
* Linked `ufeel` package
* Base cube scene setup

### Changed

* `.vscode` added to `.gitignore`
* Initial dependency configuration
