from unittest.mock import MagicMock, patch

from client_base import ClientBase


def create_client():
    client = ClientBase.__new__(ClientBase)

    client.socket = MagicMock()
    client.handlers = {}
    client.buffer = ""
    client.running = True

    return client


def test_init_connects_and_starts_thread():
    with patch("client_base.socket.socket") as socket_mock, patch("client_base.threading.Thread") as thread_mock:

        socket_instance = MagicMock()
        socket_mock.return_value = socket_instance

        ClientBase("127.0.0.1", 1234)

        socket_instance.connect.assert_called_once_with(
            ("127.0.0.1", 1234)
        )

        thread_mock.assert_called_once()


def test_init_connection_refused():
    with patch("client_base.socket.socket") as socket_mock:
        socket_instance = MagicMock()
        socket_instance.connect.side_effect = ConnectionRefusedError()
        socket_mock.return_value = socket_instance

        client = ClientBase("127.0.0.1", 1234)

        assert client.socket is None


def test_send_sends_json():
    client = create_client()

    client.send({"value": 42})

    client.socket.sendall.assert_called_once_with(
        b'{"value": 42}\n'
    )


def test_send_does_nothing_without_socket():
    client = create_client()
    client.socket = None

    client.send({"value": 42})


def test_send_handles_exception():
    client = create_client()

    client.socket.sendall.side_effect = Exception()

    client.send({"value": 42})


def test_handle_message_calls_handler():
    client = create_client()

    handler = MagicMock()
    client.handlers["test"] = handler

    client.handle_message({
        "type": "test",
        "value": "true",
    })

    handler.assert_called_once_with(True)


def test_handle_message_parses_false_string():
    client = create_client()

    handler = MagicMock()
    client.handlers["test"] = handler

    client.handle_message({
        "type": "test",
        "value": "false",
    })

    handler.assert_called_once_with(False)


def test_handle_message_non_string_value_becomes_false():
    client = create_client()

    handler = MagicMock()
    client.handlers["test"] = handler

    client.handle_message({
        "type": "test",
        "value": 123,
    })

    handler.assert_called_once_with(False)


def test_handle_message_ignores_unknown_command():
    client = create_client()

    client.handle_message({
        "type": "unknown",
        "value": True,
    })


def test_handle_message_ignores_non_dict():
    client = create_client()

    client.handle_message("invalid")


def test_handle_message_handler_exception():
    client = create_client()

    handler = MagicMock()
    handler.side_effect = Exception()

    client.handlers["test"] = handler

    client.handle_message({
        "type": "test",
        "value": "true",
    })


def test_receive_parses_json_lines():
    client = create_client()

    handler = MagicMock()
    client.handlers["hello"] = handler

    client.socket.recv.return_value = (
        b'{"type":"hello","value":"true"}\n'
    )

    client.receive()

    handler.assert_called_once_with(True)


def test_receive_invalid_json():
    client = create_client()

    client.socket.recv.return_value = b'invalid json\n'

    client.receive()


def test_receive_empty_data_closes():
    client = create_client()

    client.socket.recv.return_value = b""

    client.receive()

    assert client.running is False


def test_receive_socket_error():
    client = create_client()

    client.socket.recv.side_effect = Exception()

    client.receive()


def test_receive_without_socket():
    client = create_client()
    client.socket = None

    client.receive()


def test_receive_keeps_partial_buffer():
    client = create_client()

    handler = MagicMock()
    client.handlers["hello"] = handler

    client.socket.recv.return_value = (
        b'{"type":"hello","value":"true"}'
    )

    client.receive()

    handler.assert_not_called()


def test_close_closes_socket():
    client = create_client()

    client.close()

    assert client.running is False
    client.socket.close.assert_called_once()


def test_close_handles_exception():
    client = create_client()

    client.socket.close.side_effect = Exception()

    client.close()


def test_close_without_socket():
    client = create_client()
    client.socket = None

    client.close()

    assert client.running is False
