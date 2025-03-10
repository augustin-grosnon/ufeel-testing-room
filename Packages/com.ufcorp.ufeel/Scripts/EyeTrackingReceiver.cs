using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class EyeTrackingReceiver : MonoBehaviour
{
    private UdpClient udpClient;
    private Thread receiveThread;
    private Vector2 gazePosition = new(0.5f, 0.5f);

    void Start()
    {
        udpClient = new UdpClient(4242);
        receiveThread = new Thread(new ThreadStart(ReceiveData))
        {
            IsBackground = true
        };
        receiveThread.Start();
    }

    void ReceiveData()
    {
        IPEndPoint remoteEndPoint = new(IPAddress.Any, 4242);

        while (true)
        {
            try
            {
                byte[] receivedBytes = udpClient.Receive(ref remoteEndPoint);
                string receivedText = Encoding.ASCII.GetString(receivedBytes);
                string[] parts = receivedText.Split(',');

                if (parts.Length == 2)
                {
                    float x = float.Parse(parts[0]);
                    float y = float.Parse(parts[1]);

                    gazePosition = new Vector2(x, y);
                }
            }
            catch
            {
                // TODO: handle exceptions
            }
        }
    }

    public Vector2 GetGazePosition()
    {
        return gazePosition;
    }

    void OnApplicationQuit()
    {
        receiveThread.Abort();
        udpClient.Close();
    }
}
