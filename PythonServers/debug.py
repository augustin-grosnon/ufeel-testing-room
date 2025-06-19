import sys
import os
import site

print("===== Python Environment Debug Info =====")
print("Executable:", sys.executable)
print("Version:", sys.version)
print("Site-packages:", site.getsitepackages())
print("sys.path:", sys.path)
print("PATH:", os.environ.get("PATH", "")[:300])
print("PYTHONPATH:", os.environ.get("PYTHONPATH", ""))
print("------------------------------------------")

try:
    import cv2
    print("cv2 Version:", cv2.__version__)
    print("cv2 File:", cv2.__file__)
except Exception as e:
    print("cv2 Import Error:", e)

print("==========================================")