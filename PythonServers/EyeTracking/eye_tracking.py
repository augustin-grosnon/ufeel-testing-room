import cv2
import mediapipe as mp
import socket

UDP_IP = "127.0.0.1"
UDP_PORT = 4242
SHOW_WINDOW = False

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

mp_face_mesh = mp.solutions.face_mesh
face_mesh = mp_face_mesh.FaceMesh(
    min_detection_confidence=0.5,
    min_tracking_confidence=0.5,
    refine_landmarks=True
)

cap = cv2.VideoCapture(0)

while cap.isOpened():
    success, frame = cap.read()
    if not success:
        break

    frame = cv2.flip(frame, 1)

    rgb_frame = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)

    results = face_mesh.process(rgb_frame)

    if results.multi_face_landmarks:
        for face_landmarks in results.multi_face_landmarks:
            LEFT_EYE_CENTER = 468
            RIGHT_EYE_CENTER = 473

            left_eye = face_landmarks.landmark[LEFT_EYE_CENTER]
            right_eye = face_landmarks.landmark[RIGHT_EYE_CENTER]

            eye_x = (left_eye.x + right_eye.x) / 2
            eye_y = (left_eye.y + right_eye.y) / 2

            gaze_data = f"{eye_x},{eye_y}"
            sock.sendto(gaze_data.encode(), (UDP_IP, UDP_PORT))

            if SHOW_WINDOW:
                h, w, _ = frame.shape
                lx, ly = int(left_eye.x * w), int(left_eye.y * h)
                rx, ry = int(right_eye.x * w), int(right_eye.y * h)
                cv2.circle(frame, (lx, ly), 5, (0, 255, 0), -1)
                cv2.circle(frame, (rx, ry), 5, (0, 255, 0), -1)

    if SHOW_WINDOW:
        cv2.imshow("Eye Tracking", frame)
        if cv2.waitKey(1) & 0xFF == ord('q'):
            break

cap.release()
cv2.destroyAllWindows()
