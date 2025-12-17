# UFeel Unity Package

UFeel is an **open-source Unity plugin** designed for advanced **human-centered interaction systems**, including but not limited to **emotion detection**,  
**eye-tracking**, **speech-to-text input** and **heartbeat input** using standard webcam, microphone and external biometrics sensors.

It enables Unity developers to integrate a wide range of **natural interaction modalities** in their games and applications with minimal setup.  
A dedicated **Testing Room** (Unity scenes) is included to demonstrate the plugin’s capabilities.

---

## Project Concept & Goal

UFeel explores how **human emotions, gaze direction, voice and heart rate** can be used as real gameplay inputs.

The goals of the project are:

- Provide a **Unity-ready plugin** for emotion, eye tracking, and speech analysis.
- Provide easy **API access** for Unity developers.
- Offer a complete **Testing Room** to preview features before integrating them.
- Allow developers to create emotion-driven, gaze-controlled, or voice-interactive experiences.

Examples include:

- Controlling doors with facial expressions  
- Navigating a vehicle using gaze direction  
- Making in-game decisions through voice commands
- Displaying heart rate values

---

## ⚙️ Technologies Used

- **Unity 6 (6000.1.3f1)**
- **C#** (Unity MonoBehaviours & gameplay scripts)
- **Python Server backend** (currently doing transition to compiled libraries so it can be used to multiple game engine (Unity/Unreal/Godot), in order to simplify and improve the integration of player state detection.):
  - emotion detection
  - eye tracking
  - speech-to-text
  - biometrics sensors
- **Socket-based communication** between Unity & Python
- **Webcam-based inference models**

---


## 🛠️ Installation & Compilation

See **[`Documentation/Installation.md`](Documentation/Installation.md)**.

---

## 🔧 Coding Standards

We follow a combination of **Epic Games coding guidelines** and **UFeel internal conventions**.  
See **[`Documentation/Standards.md`](Documentation/Standards.md)**.

---

## 🏗️ Architecture Overview

High-level architecture is described in  
**[`Documentation/Architecture.md`](Documentation/Architecture.md)** and includes:

- Unity project folder structure  
- Python backend & model pipeline  
- Scene structure (TestingRoom base)  
- Submodules and code zones  

---

## 📘 API Reference

All functions available to Unity developers are documented in  
**[`Documentation/API.md`](Documentation/API.md)**.

---

## 🧪 Demo / Testing Room

Documentation for the Testing Room scenes is available in:  
**[`Documentation/TestingRoom`](Documentation/TestingRoom)**.

