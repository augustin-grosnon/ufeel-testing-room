from unittest.mock import MagicMock, patch

import numpy as np

from emotion_detector import EmotionDetector


def create_detector():
    detector = EmotionDetector.__new__(EmotionDetector)

    detector.process_enable = False
    detector.selected = []

    detector.pipeline = MagicMock()
    detector.send = MagicMock()

    return detector


def test_toggle_emotion_detection():
    detector = create_detector()

    detector.toggle_emotion_detection(True)

    assert detector.process_enable is True

    detector.toggle_emotion_detection(False)

    assert detector.process_enable is False


def test_process_disabled_does_nothing():
    detector = create_detector()

    detector.process(
        frame=np.zeros((100, 100, 3), dtype=np.uint8),
        counter=0,
        show_window=False,
    )

    detector.pipeline.predict.assert_not_called()
    detector.send.assert_not_called()


@patch("emotion_detector.Image.fromarray")
@patch("emotion_detector.cv2.cvtColor")
def test_process_without_binary_result(
    mock_cvt_color,
    mock_from_array,
):
    detector = create_detector()

    detector.process_enable = True
    detector.pipeline.predict.return_value = {}

    detector.process(
        frame=np.zeros((100, 100, 3), dtype=np.uint8),
        counter=0,
        show_window=False,
    )

    detector.send.assert_not_called()


@patch("emotion_detector.Image.fromarray")
@patch("emotion_detector.cv2.cvtColor")
def test_process_sends_emotion_data(
    mock_cvt_color,
    mock_from_array,
):
    detector = create_detector()

    detector.process_enable = True

    detector.pipeline.predict.return_value = {
        "binary": {
            "happy": True,
            "sad": False,
        },
        "heads": {
            "happy": 0.9,
            "sad": 0.1,
        },
    }

    detector.process(
        frame=np.zeros((100, 100, 3), dtype=np.uint8),
        counter=0,
        show_window=False,
    )

    detector.send.assert_called_once_with(
        {
            "happy": {
                "value": 0.9,
                "enabled": True,
            },
            "sad": {
                "value": 0.1,
                "enabled": False,
            },
        }
    )


@patch("emotion_detector.Image.fromarray")
@patch("emotion_detector.cv2.cvtColor")
def test_process_updates_selected_emotions(
    mock_cvt_color,
    mock_from_array,
):
    detector = create_detector()

    detector.process_enable = True

    detector.pipeline.predict.return_value = {
        "binary": {
            "happy": True,
        },
        "heads": {
            "happy": 1.0,
        },
    }

    detector.process(
        frame=np.zeros((100, 100, 3), dtype=np.uint8),
        counter=0,
        show_window=False,
    )

    assert detector.selected == ["happy"]


@patch("emotion_detector.cv2.putText")
@patch("emotion_detector.cv2.getTextSize")
def test_draw(
    mock_get_text_size,
    mock_put_text,
):
    detector = create_detector()

    mock_get_text_size.return_value = ((50, 20), 0)

    detector._draw(
        np.zeros((100, 100, 3), dtype=np.uint8),
        {
            "happy": 0.9,
            "sad": 0.1,
        },
        ["happy"],
    )

    assert mock_put_text.called
