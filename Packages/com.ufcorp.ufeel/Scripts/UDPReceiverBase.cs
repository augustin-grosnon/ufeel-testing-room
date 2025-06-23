using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public abstract class UDPReceiverBase
{
    public int Port;
    protected UdpClient _udpClient;
    protected Thread _receiveThread;

    protected UDPReceiverBase(int port)
    {
        Port = port;
        Setup();
        Application.quitting += OnApplicationQuit;
    }

    protected virtual void Setup()
    {
        _udpClient = new UdpClient(Port);
        _receiveThread = new Thread(ReceiveData) { IsBackground = true };
        _receiveThread.Start();
    }

    volatile bool _running = true;

    protected void ReceiveData()
    {
        IPEndPoint remoteEndPoint = new(IPAddress.Any, Port);
        while (_running)
        {
            try
            {
                byte[] receivedBytes = _udpClient.Receive(ref remoteEndPoint);
                ProcessData(receivedBytes);
            }
            catch (SocketException e)
            {
                Debug.LogWarning("SocketException: " + e.Message);
                break;
            }
            catch (System.Exception e)
            {
                Debug.LogError("Exception in ReceiveData: " + e.Message);
            }
        }
    }

    protected abstract void ProcessData(byte[] data);

    protected virtual void OnApplicationQuit()
    {
        StopReceiver();
        Application.quitting -= OnApplicationQuit;
    }

    public void StopReceiver()
    {
        _running = false;
        _udpClient?.Close();
    }
}
