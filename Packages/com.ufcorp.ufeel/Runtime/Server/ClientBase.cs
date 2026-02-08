using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class Message
{
    public string type;
    public string value;
}

public abstract class ClientBase
{
    protected int _port;
    protected TcpListener _tcpListener;
    protected Thread _clientThread;
    protected TcpClient _client;
    protected NetworkStream _stream;
    protected volatile bool _running = true;
    protected ClientBase(int port)
    {
        _port = port;
        Setup();
        Application.quitting += OnApplicationQuit;
    }

    protected virtual void Setup()
    {
        _tcpListener = new TcpListener(IPAddress.Any, _port);
        _tcpListener.Start();

        _clientThread = new Thread(ClientHandler) { IsBackground = true };
        _clientThread.Start();

        Debug.Log($"Server started on port {_port}, waiting for client connection...");
    }

    private void ClientHandler()
    {
        try
        {
            _tcpListener.Server.ReceiveTimeout = 100;
            _tcpListener.Server.SendTimeout = 100;
            while (_running)
            {
                if (_tcpListener.Pending())
                {
                    _client = _tcpListener.AcceptTcpClient();
                    break;
                }
                Thread.Sleep(10);
            }
            if (!_running)
                return;
            Debug.Log("Client connected.");
            _stream = _client.GetStream();

            byte[] buffer = new byte[4096];
            StringBuilder sb = new StringBuilder();

            while (_running && _client.Connected)
            {
                if (!_stream.DataAvailable)
                {
                    Thread.Sleep(10);
                    continue;
                }

                int bytesRead = _stream.Read(buffer, 0, buffer.Length);
                if (bytesRead <= 0)
                {
                    Debug.Log("Client disconnected.");
                    break;
                }

                sb.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));

                string dataStr = sb.ToString();
                int newlineIndex;
                while ((newlineIndex = dataStr.IndexOf('\n')) >= 0)
                {
                    string jsonMsg = dataStr.Substring(0, newlineIndex).Trim();
                    if (!string.IsNullOrEmpty(jsonMsg))
                    {
                        try
                        {
                            ProcessData(Encoding.UTF8.GetBytes(jsonMsg));
                        }
                        catch (Exception ex)
                        {
                            Debug.LogWarning($"Exception in ProcessData: {ex}");
                        }
                    }
                    dataStr = dataStr.Substring(newlineIndex + 1);
                }
                sb.Clear();
                sb.Append(dataStr);
            }
        }
        catch (ThreadAbortException)
        {
            Debug.Log("Client thread aborted by Unity (PlayMode stop).");
            Thread.ResetAbort();
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Exception in ClientHandler: {ex}");
        }
        finally
        {
            CloseClient();
        }
    }


    public static byte[] CreateData(string type, string value)
    {
        var message = new Message
        {
            type = type,
            value = value
        };

        string jsonString = JsonUtility.ToJson(message);
        return Encoding.UTF8.GetBytes(jsonString);
    }

    public void SendData(byte[] data)
    {
        if (_stream != null && _client != null && _client.Connected)
        {
            try
            {
                byte[] dataWithNewline = Encoding.UTF8.GetBytes(Encoding.UTF8.GetString(data) + "\n");
                _stream.Write(dataWithNewline, 0, dataWithNewline.Length);
                _stream.Flush();

                Debug.Log($"Sent to client: {Encoding.UTF8.GetString(data).Trim()}");
            }
            catch (Exception e)
            {
                Debug.LogWarning("Exception in SendData: " + e.Message);
            }
        }
        else
        {
            Debug.LogWarning("Cannot send data: client not connected.");
        }
    }

    protected abstract void ProcessData(byte[] data);

    protected virtual void OnApplicationQuit()
    {
        _running = false;
        CloseClient();
        _tcpListener?.Stop();
        Application.quitting -= OnApplicationQuit;
    }

    private void CloseClient()
    {
        try
        {
            _stream?.Close();
            _client?.Close();
            Debug.Log("Client connection closed.");
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Exception closing client: " + ex);
        }
    }
}
