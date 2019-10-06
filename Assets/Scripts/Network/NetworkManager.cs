using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class NetworkManager : MBSingleton<NetworkManager>, IReceiveData {
    public IPAddress ipAddress { get; private set; }

    public int port { get; private set; }

    public bool isServer { get; private set; }

    public int TimeOut = 30;

    public Action<byte[], IPEndPoint> OnReceiveEvent;

    private UdpConnection connection;

    private readonly Dictionary<uint, Client> clients = new Dictionary<uint, Client>();
    private readonly Dictionary<IPEndPoint, uint> ipToId = new Dictionary<IPEndPoint, uint>();

    public uint clientId { get; set; }

    public void StartConnection(int port) {
        this.port = port;
        connection = new UdpConnection(port, this);
    }

    public void StartConnection(IPAddress ipAddress, int port) {
        this.port = port;
        connection = new UdpConnection(ipAddress, port, this);
    }

    public void OnReceiveData(byte[] data, IPEndPoint ip) {
        if (OnReceiveEvent != null) {
            OnReceiveEvent.Invoke(data, ip);
        }
    }

    public void SendToClient(byte[] data, IPEndPoint ipEndPoint) {
        connection.Send(data, ipEndPoint);
    }

    public void SendToServer(byte[] data) {
        connection.Send(data);
    }

    public void Broadcast(byte[] data) {
        using(var iterator = clients.GetEnumerator()) {
            while (iterator.MoveNext()) {
                connection.Send(data, iterator.Current.Value.ipEndPoint);
            }
        }
    }

    void Update() {
        // We flush the data in main thread
        if (connection != null) {
            connection.FlushReceiveData();
        }
    }
}