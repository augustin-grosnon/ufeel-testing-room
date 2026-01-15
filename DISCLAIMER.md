# Disclaimer on Hardware Dependency and Data Quality

## Scope

This project provides software tools and libraries for advanced input systems, including but not limited to:

* Eye tracking
* Emotion recognition
* Voice and audio-based input
* Physiological signal processing (e.g. heart rate)

The accuracy, reliability, and consistency of these systems are **directly dependent on external hardware and environmental conditions**.

## Hardware Dependency

The tools provided by this project rely on input data captured by third-party hardware devices. These include, but are not limited to:

* Microphones
* Cameras
* Heart rate sensors and other biometric hardware

The project does **not** control the design, quality, calibration, or performance of such hardware.

As a result:

* Different devices may produce significantly different results
* Low-quality or improperly configured hardware may degrade accuracy
* Hardware limitations may introduce noise, latency, or bias into input data

## Examples of Hardware Impact

Non-exhaustive examples include:

* **Voice recognition** accuracy being affected by microphone quality, background noise, and sampling rate
* **Eye tracking** accuracy being affected by camera resolution, frame rate, lighting conditions, and camera placement
* **Emotion recognition** being affected by camera quality, facial visibility, occlusions, and cultural or individual variation
* **Heart rate and physiological signals** being affected by sensor precision, skin contact, motion artifacts, and device calibration

These effects are inherent to the hardware and environment and are not software defects.

## No Guaranteed Accuracy

The project does **not** guarantee:

* Correct detection
* Real-time accuracy
* Consistent results across devices
* Suitability for medical, psychological, or diagnostic purposes

All outputs produced by the system should be treated as **approximate signals**, not authoritative measurements.

## Responsibility of Integrators and Users

Developers and researchers integrating this project are responsible for:

* Selecting appropriate hardware for their use case
* Testing and validating behavior on target devices
* Informing end users of potential inaccuracies
* Implementing fallbacks or safeguards where incorrect input may cause issues

## Intended Use

These tools are intended for:

* Research
* Prototyping
* Experimental interaction systems
* Non-critical gameplay or software features

They are **not intended** for safety-critical, medical, or diagnostic applications.

## Summary

Hardware quality, configuration, and environment play a decisive role in the behavior of the systems provided by this project.
Variations in results are expected and unavoidable.

Users of this project must account for these limitations during development and deployment.
