import cv2


class DebugDrawer:
    def draw_landmarks(self, frame, landmarks, indices, color=(0, 255, 0)):
        for i in indices:
            x, y = landmarks[i]
            cv2.circle(frame, (x, y), 2, color, -1)

    def draw_point(self, frame, point, color=(255, 0, 0), label=None):
        x, y = point
        cv2.circle(frame, (x, y), 4, color, -1)
        if label:
            cv2.putText(frame, label, (x + 5, y - 5),
                        cv2.FONT_HERSHEY_SIMPLEX, 0.4, color, 1)
