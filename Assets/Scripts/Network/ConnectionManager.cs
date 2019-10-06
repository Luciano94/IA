﻿using System;
using System.Net;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public struct Client {
    public enum State {
        ConnectionPending,
        Connected,
    }

    public uint id;
    public ulong clientSalt;
    public ulong serverSalt;
    public IPEndPoint ipEndPoint;
    public State state;
    public float timeStamp;

    public Client(IPEndPoint ipEndPoint, uint id, float timeStamp) {
        this.ipEndPoint = ipEndPoint;
        this.id = id;
        this.clientSalt = 0;
        this.serverSalt = 0;
        this.timeStamp = timeStamp;
        this.state = State.ConnectionPending;
    }
}
 

public class ConnectionManager : MBSingleton<ConnectionManager> {
    private readonly Dictionary<uint, Client> clients = new Dictionary<uint, Client>();
    public Dictionary<uint, Client>.Enumerator ClientIterator {
        get {
            return clients.GetEnumerator();
        }
    }
    private readonly Dictionary<IPEndPoint, uint> ipToId = new Dictionary<IPEndPoint, uint>();
    private ulong clientSalt;
    private ulong serverSalt;
    private State currState;
    private const float SEND_RATE = 0.01f;
    private float timer = 0f;
    private uint clientId;
    public bool isServer { get; private set; }
    public IPAddress ipAddress { get; private set; }

    public int port { get; private set; }

    public enum State {
        SendingConnectionRequest,
        RequestingChallenge,
        RespondingChallenged,
        Connected,
    }

    protected override void Awake() {
        base.Awake();

        enabled = false;
    }

    public void StartServer(int port) {
        isServer = true;
        this.port = port;
        NetworkManager.Instance.StartConnection(port);
    }

    public void StartClient(IPAddress ip, int port) {
        isServer = false;

        this.port = port;
        this.ipAddress = ip;

        NetworkManager.Instance.StartConnection(ipAddress, port);
        currState = State.SendingConnectionRequest;
        enabled = true;
        //empezar handshake
    }
    
    void RemoveClient(IPEndPoint ip) {
        if (ipToId.ContainsKey(ip)) {
            Debug.Log("Removing client  + ip.Address");
            clients.Remove(ipToId[ip]);
        }
    }

    private void Update() {
        if (!isServer) {
            timer += Time.unscaledDeltaTime;
            
            if (timer >= SEND_RATE) {
                switch (currState) {
                    case State.SendingConnectionRequest: {
                        SendConnectionRequest();
                    } break;
                    case State.RespondingChallenged: {
                        SendChallengeResponse(clientSalt, serverSalt);
                    } break;
                }
                timer = 0f;
            }
        }
    }

    private void SendToServer<T>(NetworkPacket<T> packet) {
        PacketManager.Instance.SendPacket<T>(packet);
    }

    private void SendToClient<T>(NetworkPacket<T> packet, IPEndPoint ipEndPoint) {
        PacketManager.Instance.SendPacket<T>(packet, ipEndPoint);
    }

    private void SendConnectionRequest() {
        ConnectionRequestPacket request = new ConnectionRequestPacket();
        request.payload.clientSalt = LongRandom.GetRandom();
        SendToServer(request);
    }

    private void CheckAndSendChallengeRequest(IPEndPoint ipEndpoint, ConnectionRequestData connectionRequestData) {
        if (isServer) {
            if (!ipToId.ContainsKey(ipEndpoint)) {
                Client newClient = new Client(ipEndpoint, clientId++, DateTime.Now.Ticks);
                newClient.clientSalt = connectionRequestData.clientSalt;
                newClient.serverSalt = LongRandom.GetRandom();
                clients.Add(newClient.id, newClient);
                ipToId.Add(ipEndpoint, newClient.id);
            }
            SendChallengeRequest(clients[ipToId[ipEndpoint]]);
        }
    }

    private void SendChallengeRequest(Client client) {
        ChallengeRequestPacket request = new ChallengeRequestPacket();
        request.payload.clientId = client.id;
        request.payload.clientSalt = client.clientSalt;
        request.payload.serverSalt = client.serverSalt;
        SendToClient(request, client.ipEndPoint);
    }

    private void CheckAndSendChallengeResponse(IPEndPoint ipEndpoint, ChallengeRequestData challengeRequestData) {
        if (!isServer && currState == State.SendingConnectionRequest) {
            clientSalt = challengeRequestData.clientSalt;
            serverSalt = challengeRequestData.serverSalt;
            currState = State.RespondingChallenged;
            SendChallengeResponse(clientSalt, serverSalt);
        }
    }
    

    private void SendChallengeResponse(ulong clientSalt, ulong serverSalt) {
        ChallengeResponsePacket request = new ChallengeResponsePacket();
        request.payload.result = clientSalt ^ serverSalt;
        SendToServer(request);
    }

    private void CheckResult(IPEndPoint ipEndPoint, ChallengeResponseData challengeResponseData) {
        if (isServer) {
            Client client = clients[ipToId[ipEndPoint]];
            ulong result = client.clientSalt ^ client.serverSalt;
            if (challengeResponseData.result == result) {
                SendToClient(new ConnectedPacket(), ipEndPoint);
            }
        }
    }

    private void FinishHandShake() {
        if (!isServer && currState == State.RespondingChallenged) {
            currState = State.Connected;
            enabled = false;
        }
    }
    
    public void OnReceivePacket(IPEndPoint ipEndpoint, PacketType packetType, Stream stream)
    {
        switch (packetType)
        {
            case PacketType.ConnectionRequest: {
                ConnectionRequestPacket connectionRequestPacket = new ConnectionRequestPacket();
                connectionRequestPacket.Deserialize(stream);
                CheckAndSendChallengeRequest(ipEndpoint, connectionRequestPacket.payload);
            } break;
            case PacketType.ChallengeRequest: {
                ChallengeRequestPacket challengeRequestPacket = new ChallengeRequestPacket();
                challengeRequestPacket.Deserialize(stream);
                CheckAndSendChallengeResponse(ipEndpoint, challengeRequestPacket.payload);
            } break;
            case PacketType.ChallengeResponse: {
                ChallengeResponsePacket challengeResponsePacket = new ChallengeResponsePacket();
                challengeResponsePacket.Deserialize(stream);
                CheckResult(ipEndpoint, challengeResponsePacket.payload);
            } break;
            case PacketType.Connected: {
                FinishHandShake();
            } break;
        }
    }
}