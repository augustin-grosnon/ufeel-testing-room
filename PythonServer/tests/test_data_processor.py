from unittest.mock import MagicMock, patch
import sys


@patch("data_processor.cv2.destroyAllWindows")
@patch("data_processor.time.sleep")
@patch("data_processor.time.time")
@patch("cv2.waitKey")
@patch("cv2.imshow")
@patch("data_processor.cv2.resize")
@patch("cv2.flip")
@patch("data_processor.cv2.VideoCapture")
@patch("data_processor.EmotionDetector")
@patch("data_processor.EyeTracker")
@patch("data_processor.SpeechToText")
@patch("data_processor.HeartRateSensor")
def test_data_processor_process_one_frame(
    mock_hr,
    mock_stt,
    mock_eye,
    mock_emotion,
    mock_camera,
    mock_flip,
    mock_resize,
    mock_imshow,
    mock_waitkey,
    mock_time,
    mock_sleep,
    mock_destroy_windows,
):
    from data_processor import DataProcessor

    frame = MagicMock()

    camera = MagicMock()
    camera.isOpened.side_effect = [True, False]
    camera.read.return_value = (True, frame)

    mock_camera.return_value = camera

    mock_flip.return_value = frame
    mock_waitkey.return_value = ord("p")

    processor = DataProcessor(
        calibration=False,
        show_window=True,
    )

    processor.process()

    mock_emotion.return_value.process.assert_called_once()
    mock_eye.return_value.process.assert_called_once()
    mock_stt.return_value.process.assert_called_once()
    mock_hr.return_value.process.assert_called_once()

    camera.release.assert_called_once()
