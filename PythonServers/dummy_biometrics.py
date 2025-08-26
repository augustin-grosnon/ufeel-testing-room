from http.server import BaseHTTPRequestHandler, HTTPServer
from random import randint
import time

hostName = "localhost"
serverPort = 4321

class MyServer(BaseHTTPRequestHandler):
    def do_GET(self):
        n = randint(50, 200)
        self.send_response(200)
        self.send_header("Content-type", "application/json")
        self.end_headers()
        self.wfile.write(bytes("{\"bpm\": %s}" % n, "utf-8"))

if __name__ == "__main__":
    webServer = HTTPServer((hostName, serverPort), MyServer)
    print("Server started http://%s:%s" % (hostName, serverPort))

    try:
        webServer.serve_forever()
    except KeyboardInterrupt:
        pass

    webServer.server_close()
    print("Server stopped.")
