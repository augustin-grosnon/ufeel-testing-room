from http.server import BaseHTTPRequestHandler, HTTPServer

class GetHandler(BaseHTTPRequestHandler):
    def do_GET(self):
        self.send_response(200)
        self.send_header('Content-Type', 'application/json')
        self.end_headers()
        self.wfile.write(b'{"bpm":150}')

HTTPServer(('localhost', 8000), GetHandler).serve_forever()
