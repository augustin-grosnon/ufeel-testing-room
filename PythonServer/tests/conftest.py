import sys
from unittest.mock import MagicMock

sys.modules["facenet_pytorch"] = MagicMock()
sys.modules["vosk"] = MagicMock()
