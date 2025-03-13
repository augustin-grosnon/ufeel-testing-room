using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public abstract class UDPReceiverBase : MonoBehaviour
{
    public int port;

    protected UdpClient udpClient;
    protected Thread receiveThread;

    protected virtual void Start()
    {
        Setup();
    }

    protected virtual void Setup()
    {
        udpClient = new UdpClient(port);
        receiveThread = new Thread(ReceiveData) { IsBackground = true };
        receiveThread.Start();
    }

    protected void ReceiveData()
    {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, port);
        while (true)
        {
            try
            {
                byte[] receivedBytes = udpClient.Receive(ref remoteEndPoint);
                ProcessData(receivedBytes);
            }
            catch (ThreadAbortException)
            {
                break;
            }
            catch (SocketException e)
            {
                Debug.LogWarning("SocketException: " + e.Message);
                break;
            }
            catch (System.Exception e)
            {
                Debug.Log("Exception in ReceiveData: " + e.Message);
            }
        }
    }

    protected abstract void ProcessData(byte[] data);

    protected virtual void OnApplicationQuit()
    {
        if (receiveThread != null && receiveThread.IsAlive)
            receiveThread.Abort();
        if (udpClient != null)
            udpClient.Close();
    }
}
