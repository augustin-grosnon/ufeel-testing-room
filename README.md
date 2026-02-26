# UFeel Testing Room

A Unity project demonstrating the [UFeel](Packages/com.ufcorp.ufeel) input package - emotion detection, eye tracking, speech recognition, and heart rate sensing as Unity gameplay inputs.

## Documentation

| Document | Description |
|---|---|
| [Architecture](ARCHITECTURE.md) | System architecture and component overview |
| [API Reference](Packages/com.ufcorp.ufeel/Runtime/API.md) | UFeel Unity API usage guide |
| [Testing Room](Documentation/TestingRoom/README.md) | Testing Room project overview |
| [Setup Instructions](Documentation/TestingRoom/SetupInstructions.md) | How to run the project |
| [Scene Reference](Documentation/TestingRoom/Scenes.md) | Scene-by-scene breakdown |
| [Testing & Debugging](Documentation/TestingRoom/TestingTips.md) | Testing and debugging guide |
| [Changelog](CHANGELOG.md) | Release history |
| [Contributing](CONTRIBUTING.md) | Contribution guidelines |
| [Disclaimer](DISCLAIMER.md) | Limitations and prohibited use |

## Quick Start

1. Clone the repository.
2. Open in Unity Hub (TODO: add version).
3. Open the `TestingRoom` scene and press Play.

A webcam is required for most features.

## Build Modes

This project is intended to support two build profiles:

* **Development** – enables debug tools, extended logging, and testing utilities.
* **Production** – excludes development-only code and debug systems.

Separation will be handled using Unity Build Profiles and a `DEV` scripting define symbol.

Code wrapped in:

```csharp
#if DEV
// development-only code
#endif
```

will only be compiled in Development builds.

This setup is currently only planned and may be automated in a future update.
TODO: implement in the project
TODO: update this README

## License

Apache License 2.0 - see [LICENSE](LICENSE).
