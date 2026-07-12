import mediapipe as mp
import cv2


class FaceMeshDetector:
    def __init__(self):
        self.mp_face_mesh = mp.solutions.face_mesh
        self.face_mesh = self.mp_face_mesh.FaceMesh(
            static_image_mode=False,
            max_num_faces=1,
            refine_landmarks=True,
            min_detection_confidence=0.5,
            min_tracking_confidence=0.5,
        )

    def process(self, frame):
        rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        return self.face_mesh.process(rgb)

    def get_landmarks(self, results, frame_shape):
        if not results.multi_face_landmarks:
            return None

        h, w = frame_shape[:2]
        landmarks = results.multi_face_landmarks[0].landmark

        return [(int(lm.x * w), int(lm.y * h)) for lm in landmarks]
