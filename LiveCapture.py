import cv2
import mediapipe as mp
from pythonosc import udp_client

# INicia classes Mediapipe e OpenCV
mp_pose = mp.solutions.pose
pose = mp_pose.Pose()
mp_drawing = mp.solutions.drawing_utils

# OSCclient manda info para Unity
client = udp_client.SimpleUDPClient("127.0.0.1", 8000)

cap = cv2.VideoCapture(0) # captura com camera

while cap.isOpened():
    ret, frame = cap.read()
    image = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
    results = pose.process(image)

    # Converte para rendering OpenCV
    image = cv2.cvtColor(image, cv2.COLOR_RGB2BGR)

    if results.pose_landmarks:
        # Envia info da pose para o Unity via OSC
        for idx, landmark in enumerate(results.pose_landmarks.landmark):
            # flipped_y = 1.0 - landmark.y # flip dos axis para o Unity (sistema de coordenadas diferentes)
            
            # Envia cada ponto como coordenadas x, y, z
            client.send_message(f"/pose/landmark_{idx}", [landmark.x, landmark.y, landmark.z])

    # desenha os pontos captados por cima da captura
    mp_drawing.draw_landmarks(image, results.pose_landmarks, mp_pose.POSE_CONNECTIONS)
    cv2.imshow('Pose Estimation', image)

    if cv2.waitKey(10) & 0xFF == ord('q'): # tecla "q" para fechar programa
        break

cap.release()
cv2.destroyAllWindows()