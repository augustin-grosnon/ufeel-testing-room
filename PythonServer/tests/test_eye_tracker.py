from unittest.mock import MagicMock, patch

import numpy as np

from eye_tracker import EyeTracker, EyeTrackingError


def create_tracker():
    tracker = EyeTracker.__new__(EyeTracker)

    tracker.process_enable = False

    tracker.detector = MagicMock()
    tracker.analyzer = MagicMock()
    tracker.drawer = MagicMock()

    tracker.send = MagicMock()

    tracker.blink_state = {
        "counter": 0,
        "is_blinking": False,
        "threshold_frames": 3,
    }

    tracker.gaze_check = {
        "right": lambda x: x > 0.55,
        "left": lambda x: x < 0.45,
        "center": lambda x: 0.45 <= x <= 0.55,
    }

    tracker.gaze_buffer = []

    return tracker


def test_blink_requires_three_frames():
    tracker = create_tracker()

    assert tracker._update_blink_state(True) is False
    assert tracker._update_blink_state(True) is False
    assert tracker._update_blink_state(True) is True


def test_blink_resets():
    tracker = create_tracker()

    for _ in range(3):
        tracker._update_blink_state(True)

    assert tracker._update_blink_state(False) is False
    assert tracker.blink_state["counter"] == 0


def test_gaze_average():
    tracker = create_tracker()

    assert tracker._update_gaze_state(0.5) == 0.5
    assert tracker._update_gaze_state(0.7) == 0.6


def test_toggle_eye_detection():
    tracker = create_tracker()

    tracker.toggle_eye_detection(True)

    assert tracker.process_enable is True

    tracker.toggle_eye_detection(False)

    assert tracker.process_enable is False


def test_disabled_tracker_does_nothing():
    tracker = create_tracker()

    tracker.process(
        frame=None,
        calibration=False,
        show_window=False,
    )

    tracker.send.assert_not_called()


def test_no_landmarks_sends_error():
    tracker = create_tracker()

    tracker.process_enable = True

    tracker.detector.process.return_value = None
    tracker.detector.get_landmarks.return_value = None

    tracker.process(
        frame=np.zeros((100, 100, 3), dtype=np.uint8),
        calibration=False,
        show_window=False,
    )

    tracker.send.assert_called_once_with(
        {
            "error": EyeTrackingError.NO_EYES_DETECTED.value
        }
    )


def test_process_sends_eye_data():
    tracker = create_tracker()

    tracker.process_enable = True

    landmarks = [(0, 0)] * 477

    tracker.detector.process.return_value = "results"
    tracker.detector.get_landmarks.return_value = landmarks

    tracker.analyzer.detect_gaze.return_value = 0.5
    tracker.analyzer.detect_blink.return_value = False

    tracker.process(
        frame=np.zeros((100, 100, 3), dtype=np.uint8),
        calibration=False,
        show_window=False,
    )

    tracker.send.assert_called_once_with(
        {
            "right": False,
            "left": False,
            "center": True,
            "blink": False,
        }
    )


def test_get_landmarks_calls_detector():
    tracker = create_tracker()

    tracker.detector.process.return_value = "results"
    tracker.detector.get_landmarks.return_value = ["landmarks"]

    result = tracker._get_landmarks(
        np.zeros((10, 10, 3), dtype=np.uint8)
    )

    assert result == ["landmarks"]

    tracker.detector.process.assert_called_once()
    tracker.detector.get_landmarks.assert_called_once()


@patch("eye_tracker.cv2.putText")
@patch("eye_tracker.cv2.circle")
def test_draw_calls_opencv(
    mock_circle,
    mock_put_text,
):
    tracker = create_tracker()

    landmarks = [(1, 1)] * 477

    tracker._draw(
        np.zeros((100, 100, 3), dtype=np.uint8),
        0.5,
        False,
        landmarks,
    )

    assert mock_put_text.called
    assert mock_circle.called


def test_draw_without_drawer():
    tracker = create_tracker()

    tracker.drawer = None

    tracker._draw(
        np.zeros((100, 100, 3), dtype=np.uint8),
        0.5,
        False,
        [(1, 1)] * 477,
    )
