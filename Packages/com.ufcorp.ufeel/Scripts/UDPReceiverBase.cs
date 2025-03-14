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
        this.Port = port;
        Setup();
        Application.quitting += OnApplicationQuit;
    }

    protected virtual void Setup()
    {
        _udpClient = new UdpClient(Port);
        _receiveThread = new Thread(ReceiveData) { IsBackground = true };
        _receiveThread.Start();
    }

    protected void ReceiveData()
    {
        IPEndPoint remoteEndPoint = new(IPAddress.Any, Port);
        while (true)
        {
            try
            {
                byte[] receivedBytes = _udpClient.Receive(ref remoteEndPoint);
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
        StopReceiver();
        Application.quitting -= OnApplicationQuit;
    }

    public void StopReceiver()
    {
        if (_receiveThread != null && _receiveThread.IsAlive)
            _receiveThread.Abort();
        _udpClient?.Close();
    }
}
