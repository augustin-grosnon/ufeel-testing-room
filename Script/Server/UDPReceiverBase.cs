using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public abstract class UDPReceiverBase
{
    public int Port;
    protected UdpClient? _udpClient;
    protected Thread? _receiveThread;

    protected UDPReceiverBase(int port)
    {
        Port = port;
        Setup();
        Application.quitting += OnApplicationQuit;
    }

    protected virtual void Setup()
    {
        var udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        udpSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

        udpSocket.Bind(new IPEndPoint(IPAddress.Any, Port));

        _udpClient = new UdpClient { Client = udpSocket };
        _receiveThread = new Thread(ReceiveData) { IsBackground = true };
        _receiveThread.Start();
    }

    protected void ReceiveData()
    {
        Debug.Log("Salut toi");
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, Port);
        while (true)
        {
            Debug.Log("Looping");
            try
            {
                byte[]? receivedBytes = _udpClient?.Receive(ref remoteEndPoint);
                Debug.Log("He narvalo on va te retrouver");
                if (receivedBytes != null)
                {
                    Debug.Log("ReceivedBytes: " + receivedBytes);
                    ProcessData(receivedBytes);
                }
            }
            catch (ThreadAbortException e)
            {
                Debug.LogWarning("ThreadAbortException: " + e.Message);
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
    public void SendData(byte[] data)
    {
        return;
        if (_udpClient == null)
        {
            Debug.LogWarning("_udpClient not initialized");
            return;
        }

        IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Port);
        _udpClient.Send(data, data.Length, serverEndpoint);
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
            _receiveThread.Join();
        _udpClient?.Close();
        _udpClient = null;
    }
}
