using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

[System.Serializable]
public class EmotionData
{
    public float happiness;
    public float surprise;
    public float sadness;
    public float anger;
    public float disgust;
    public float fear;
}

public class EmotionReceiver : MonoBehaviour
{
    public EmotionServerController emotionController;

    private UdpClient udpClient;
    private Thread receiveThread;
    public EmotionData emotionData = new();

    void Start()
    {
        if (emotionController != null)
        {
            emotionController.EnsureServerRunning();
        }
        else
        {
            Debug.LogWarning("EmotionServerController reference not set in EmotionReceiver.");
        }

        udpClient = new UdpClient(4243);
        receiveThread = new Thread(ReceiveData) { IsBackground = true };
        receiveThread.Start();
    }

    void ReceiveData()
    {
        IPEndPoint remoteEndPoint = new(IPAddress.Any, 4243);
        while (true)
        {
            byte[] receivedBytes = udpClient.Receive(ref remoteEndPoint);
            string json = Encoding.ASCII.GetString(receivedBytes);
            try
            {
                emotionData = JsonUtility.FromJson<EmotionData>(json);
            }
            catch (System.Exception e)
            {
                Debug.Log("Error parsing emotion JSON: " + e.Message);
            }
        }
    }

    void OnApplicationQuit()
    {
        receiveThread?.Abort();
        udpClient?.Close();
    }
}
