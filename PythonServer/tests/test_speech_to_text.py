from unittest.mock import MagicMock, patch

from speech_to_text import SpeechToText


def create_speech():
    speech = SpeechToText.__new__(SpeechToText)

    speech.process_enable = False
    speech.current_text = "None"
    speech.thread = None
    speech.stop_event = MagicMock()

    speech.send = MagicMock()

    return speech


def test_toggle_speech_detection():
    speech = create_speech()

    speech.toggle_speech_detection(True)

    assert speech.process_enable is True

    speech.toggle_speech_detection(False)

    assert speech.process_enable is False


def test_process_disabled_stops_thread():
    speech = create_speech()

    speech._stop_thread = MagicMock()

    speech.process(
        frame=None,
        show_window=False,
    )

    speech._stop_thread.assert_called_once()


def test_process_enabled_starts_thread():
    speech = create_speech()

    speech.process_enable = True
    speech._start_thread = MagicMock()

    speech.process(
        frame=None,
        show_window=False,
    )

    speech._start_thread.assert_called_once()


def test_process_enabled_draws_text():
    speech = create_speech()

    speech.process_enable = True
    speech._start_thread = MagicMock()
    speech.draw_centered_text_bottom = MagicMock()

    speech.process(
        frame="frame",
        show_window=True,
    )

    speech.draw_centered_text_bottom.assert_called_once_with(
        "frame",
        "None",
    )


def test_start_thread_creates_thread():
    speech = create_speech()

    speech._run_audio_loop = MagicMock()

    with patch("speech_to_text.threading.Thread") as thread:
        thread_instance = MagicMock()
        thread.return_value = thread_instance

        speech._start_thread()

        thread.assert_called_once_with(
            target=speech._run_audio_loop,
            daemon=True,
        )

        thread_instance.start.assert_called_once()


def test_start_thread_does_not_duplicate():
    speech = create_speech()

    speech.thread = MagicMock()
    speech.thread.is_alive.return_value = True

    with patch("speech_to_text.threading.Thread") as thread:
        speech._start_thread()

        thread.assert_not_called()


def test_stop_thread():
    speech = create_speech()

    thread = MagicMock()
    speech.thread = thread

    speech._stop_thread()

    speech.stop_event.set.assert_called_once()
    thread.join.assert_called_once()
    assert speech.thread is None


def test_stop_thread_without_thread():
    speech = create_speech()

    speech._stop_thread()

    speech.stop_event.set.assert_not_called()


def test_callback_updates_text():
    speech = create_speech()

    speech.recognizer = MagicMock()
    speech.recognizer.AcceptWaveform.return_value = True
    speech.recognizer.Result.return_value = '{"text":"bonjour"}'

    speech._callback(
        b"audio",
        None,
        None,
        None,
    )

    assert speech.current_text == "bonjour"

    speech.send.assert_called_once_with(
        {"text": "bonjour"}
    )


def test_callback_ignores_empty_text():
    speech = create_speech()

    speech.recognizer = MagicMock()
    speech.recognizer.AcceptWaveform.return_value = True
    speech.recognizer.Result.return_value = '{"text":""}'

    speech._callback(
        b"audio",
        None,
        None,
        None,
    )

    assert speech.current_text == "None"

    speech.send.assert_not_called()


def test_callback_ignores_invalid_waveform():
    speech = create_speech()

    speech.recognizer = MagicMock()
    speech.recognizer.AcceptWaveform.return_value = False

    speech._callback(
        b"audio",
        None,
        None,
        None,
    )

    speech.send.assert_not_called()


def test_audio_loop_handles_exception():
    speech = create_speech()

    with patch(
        "speech_to_text.sd.RawInputStream",
        side_effect=Exception("audio error"),
    ):
        speech._run_audio_loop()
