import socket
import logging
import json
import threading

logging.basicConfig(
    filename="client_base.log",
    filemode="a",
    format="%(asctime)s - %(levelname)s - %(message)s",
    level=logging.DEBUG
)

class ClientBase:
    def __init__(self, server_ip, server_port):
        self.server_ip = server_ip
        self.server_port = server_port
        self.socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.handlers = {}
        self.buffer = ""
        self.running = True

        try:
            self.socket.connect((self.server_ip, self.server_port))
            logging.info(f"Connected to server at {self.server_ip}:{self.server_port}")
        except ConnectionRefusedError as e:
            logging.error(f"Connection refused: {e}")
            self.socket = None

        if self.socket:
            self.listen_thread = threading.Thread(target=self.listen, daemon=True)
            self.listen_thread.start()

    def send(self, obj: object):
        if self.socket:
            try:
                message = json.dumps(obj) + "\n"
                self.socket.sendall(message.encode("utf-8"))
                logging.info(f"Sent: {message.strip()}")
            except Exception as e:
                logging.error(f"Error sending JSON: {e}")

    def receive(self, bufsize=4096):
        if not self.socket:
            return
        try:
            data = self.socket.recv(bufsize)
            if not data:
                logging.warning("No data received (socket may be closed)")
                self.close()
                return

            self.buffer += data.decode("utf-8")
            while '\n' in self.buffer:
                line, self.buffer = self.buffer.split('\n', 1)
                if line.strip():
                    try:
                        msg = json.loads(line.strip())
                        self.handle_message(msg)
                    except json.JSONDecodeError as e:
                        logging.error(f"Invalid JSON received: {e}")
        except Exception as e:
            logging.error(f"Error receiving message: {e}")

    def handle_message(self, msg: dict):
        if not isinstance(msg, dict):
            logging.warning("Received non-dict message")
            return
        command_type = msg.get("type")
        if command_type in self.handlers:
            try:
                command_value_str = msg.get("value")
                if isinstance(command_value_str, str):
                    command_value = command_value_str.lower() == "true"
                else:
                    command_value = False
                self.handlers[command_type](command_value)
            except Exception as e:
                logging.error(f"Error handling command '{command_type}': {e}")
        else:
            logging.warning(f"No handler found for command: {command_type}")

    def listen(self):
        while self.running and self.socket:
            logging.info("hihi")
            self.receive()

    def close(self):
        self.running = False
        if self.socket:
            try:
                self.socket.close()
                logging.info("Socket closed")
            except Exception as e:
                logging.error(f"Error closing socket: {e}")
