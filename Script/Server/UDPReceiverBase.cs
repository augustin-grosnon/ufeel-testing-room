using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using System.Text;


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
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, Port);
        while (true)
        {
            try
            {
                byte[]? receivedBytes = _udpClient?.Receive(ref remoteEndPoint);
                if (receivedBytes != null)
                {
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
        UdpClient sender = new UdpClient();

        if (sender == null)
        {
            Debug.LogWarning("sender not initialized");
            return;
        }

        string json = Encoding.ASCII.GetString(data);
        Debug.Log("Raw bytes: " + BitConverter.ToString(data));
        Debug.Log("Sending to port " + (Port - 1) + ": " + json);
        sender.Send(data, data.Length, "127.0.0.1", Port - 1);
        sender.Close();
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
