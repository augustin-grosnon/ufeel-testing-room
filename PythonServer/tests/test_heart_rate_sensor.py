from unittest.mock import MagicMock

from heart_rate_sensor import (
    HeartRateGenerator,
    HeartRateSensor,
)


def test_simulated_heart_rate_stays_in_limits():
    generator = HeartRateGenerator(
        start=90,
        min_hr=55,
        max_hr=160,
    )

    for _ in range(100):
        value = generator.get_simulated()

        assert 55 <= value <= 160


def test_simulated_heart_rate_step_limit():
    generator = HeartRateGenerator(
        start=90,
        max_step=3,
    )

    previous = generator.value

    current = generator.get_simulated()

    assert abs(current - previous) <= 3


def test_toggle_heart_rate_detection():
    sensor = HeartRateSensor.__new__(HeartRateSensor)

    sensor.process_enable = False

    sensor.toggle_heart_rate_detection(True)

    assert sensor.process_enable is True

    sensor.toggle_heart_rate_detection(False)

    assert sensor.process_enable is False


def test_change_data_source():
    sensor = HeartRateSensor.__new__(HeartRateSensor)

    sensor.hr_gen = MagicMock()

    sensor.change_data_source(True)

    assert sensor.hr_gen.source is True


def test_disabled_sensor_does_not_send():
    sensor = HeartRateSensor.__new__(HeartRateSensor)

    sensor.process_enable = False
    sensor.send = MagicMock()

    sensor.process(
        frame=None,
        counter=0,
        show_window=False,
    )

    sensor.send.assert_not_called()


def test_simulated_sensor_sends_value():
    sensor = HeartRateSensor.__new__(HeartRateSensor)

    sensor.process_enable = True
    sensor.current_heart_rate = -1

    sensor.hr_gen = MagicMock()
    sensor.hr_gen.source = True
    sensor.hr_gen.get_simulated.return_value = 80

    sensor.send = MagicMock()

    sensor.process(
        frame=None,
        counter=0,
        show_window=False,
    )

    sensor.send.assert_called_once_with(
        {"rate": 80}
    )
