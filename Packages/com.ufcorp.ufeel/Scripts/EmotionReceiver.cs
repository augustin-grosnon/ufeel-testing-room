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
    public EmotionController emotionController;

    UdpClient udp;
    Thread receiveThread;
    public EmotionData emotionData = new EmotionData();

    void Start()
    {
        if (emotionController != null)
        {
            emotionController.EnsureServerRunning();
        }
        else
        {
            Debug.LogWarning("EmotionController reference not set in EmotionReceiver.");
        }

        udp = new UdpClient(5005);
        receiveThread = new Thread(ReceiveData) { IsBackground = true };
        receiveThread.Start();
    }

    void ReceiveData()
    {
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 5005);
        while (true)
        {
            byte[] data = udp.Receive(ref remoteEP);
            string json = Encoding.ASCII.GetString(data);
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
        udp?.Close();
    }
}
