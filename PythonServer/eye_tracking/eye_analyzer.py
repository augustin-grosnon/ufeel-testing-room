import numpy as np


class EyeAnalyzer:
    LEFT_IRIS = [468, 469, 470, 471]
    RIGHT_IRIS = [473, 474, 475, 476]

    LEFT_EYE_CORNERS = (33, 133)
    RIGHT_EYE_CORNERS = (362, 263)

    LEFT_EYE_EAR = [33, 160, 158, 133, 153, 144]
    RIGHT_EYE_EAR = [362, 385, 387, 263, 373, 380]

    def __init__(self, debug: bool = False):
        self.gaze_threshold = 0.12
        self.blink_threshold = 0.2
        self.debug = debug

    def _np(self, p):
        return np.array(p)

    def _iris_center(self, landmarks, indices):
        pts = [self._np(landmarks[i]) for i in indices]
        return np.mean(pts, axis=0)

    def _eye_ratio(self, iris, c_l, c_r):
        c_l = self._np(c_l)
        c_r = self._np(c_r)
        iris = self._np(iris)

        eye_vec = c_r - c_l
        eye_len = np.linalg.norm(eye_vec)

        if eye_len == 0:
            return 0.5

        iris_vec = iris - c_l

        ratio = np.dot(iris_vec, eye_vec) / (eye_len ** 2)

        return ratio

    def detect_gaze(self, landmarks):
        left_iris = self._iris_center(landmarks, self.LEFT_IRIS)
        right_iris = self._iris_center(landmarks, self.RIGHT_IRIS)

        left_ratio = self._eye_ratio(
            left_iris,
            landmarks[self.LEFT_EYE_CORNERS[0]],
            landmarks[self.LEFT_EYE_CORNERS[1]],
        )

        right_ratio = self._eye_ratio(
            right_iris,
            landmarks[self.RIGHT_EYE_CORNERS[0]],
            landmarks[self.RIGHT_EYE_CORNERS[1]],
        )

        gaze = (left_ratio + right_ratio) / 2.0

        gaze = float(np.clip(gaze, 0.0, 1.0))

        if self.debug:
            print("GAZE RAW:", gaze)

        return gaze

    def _ear(self, landmarks, eye):
        p1 = self._np(landmarks[eye[0]])
        p2 = self._np(landmarks[eye[1]])
        p3 = self._np(landmarks[eye[2]])
        p4 = self._np(landmarks[eye[3]])
        p5 = self._np(landmarks[eye[4]])
        p6 = self._np(landmarks[eye[5]])

        v1 = np.linalg.norm(p2 - p6)
        v2 = np.linalg.norm(p3 - p5)
        h = np.linalg.norm(p1 - p4)

        if h == 0:
            return 1.0

        return (v1 + v2) / (2.0 * h)

    def detect_blink(self, landmarks):
        left_ear = self._ear(landmarks, self.LEFT_EYE_EAR)
        right_ear = self._ear(landmarks, self.RIGHT_EYE_EAR)

        avg_ear = (left_ear + right_ear) / 2.0

        if self.debug:
            print("--- BLINK DEBUG ---")
            print("left:", left_ear, "right:", right_ear, "avg:", avg_ear)

        return avg_ear < self.blink_threshold
