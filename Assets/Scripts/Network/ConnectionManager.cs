using System.Net;
using System.Collections.Generic;
using UnityEngine;

public struct Client {
    public enum State {
        ConnectionPending,
        Connected,
    }

    public uint id;
    public long clientSalt;
    public long serverSalt;
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
    private long clientSalt;
    private State currState;
    private const float SEND_RATE = 0.1f;
    private float timer = 0f;
    private Client client;
    private uint clientId;
    public bool isServer { get; private set; }
    public IPAddress ipAddress { get; private set; }

    public int port { get; private set; }

    public enum State {
        ConnectionSent,
        ChallengeRequested,
        ChallengedResponded,
        Connected,
    }

    public ConnectionManager(IPEndPoint ipEndPoint) {
        isServer = false;
        ipEndPoint = new IPEndPoint(ipAddress, port);
        StartClient(ipAddress, port);
        //client = new Client(ipEndPoint);
    }

    public ConnectionManager() {
        isServer = true;
    }


    private void Awake() {
        //PacketManager.Instance.AddListener((uint)gameObject.GetInstanceID(), );
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

        //empezar handshake
    }
    
    void AddClient(IPEndPoint ip, uint id) {
        if (isServer && !ipToId.ContainsKey(ip)) {
            Debug.Log("Adding client  + ip.Address");

            ipToId[ip] = id;

            clients.Add(id, new Client(ip, id, Time.realtimeSinceStartup));
        }
    }

    void RemoveClient(IPEndPoint ip) {
        if (ipToId.ContainsKey(ip)) {
            Debug.Log("Removing client  + ip.Address");
            clients.Remove(ipToId[ip]);
        }
    }


    private void Update() {
        timer += Time.deltaTime;
        
        if (timer >= SEND_RATE) {
            switch (currState) {
                case State.ConnectionSent: {
                    ConnectionRequest();
                } break;
            }
            timer = 0f;
        }
    }

    private void SendToServer<T>(NetworkPacket<T> packet) {
        PacketManager.Instance.SendPacket<T>(packet);
    }

    private void SendToClient<T>(NetworkPacket<T> packet, IPEndPoint ipEndPoint) {
        PacketManager.Instance.SendPacket<T>(packet, ipEndPoint);
    }

    private void ConnectionRequest() {
        if (!isServer) {
            clientSalt = (long)Random.Range(0, int.MaxValue)*(long)Random.Range(0, int.MaxValue);
            SendConnectionRequest();
        }
    }

    private void SendConnectionRequest() {
        ConnectionRequestPacket request = new ConnectionRequestPacket();
        request.payload.clientSalt = client.clientSalt;
        SendToServer(request);
    }

    private void SendChallengeRequest(ConnectionRequestData connectionRequestData, 
                                      long clientId, 
                                      long clientSalt, long serverSalt, 
                                      IPEndPoint ipEndPoint) {

        ChallengeRequestPacket request = new ChallengeRequestPacket();
        request.payload.clientId = clientId;
        request.payload.clientSalt = clientSalt;
        request.payload.serverSalt = serverSalt;
        SendToClient(request, ipEndPoint);
    }

    
    private void RecieveConnectionRequest(ConnectionRequestPacket packet) {
        if (isServer) {
            ConnectionRequestData requestData = packet.payload;
            long serverSalt = (long)Random.Range(0, int.MaxValue)*(long)Random.Range(0, int.MaxValue);
        
            SendChallengeRequest(requestData, clientId, requestData.clientSalt,);
            ++clientId;
        }
    }

    private void SendChallengeResponse(long clientSalt, long serverSalt) {
        ChallengeResponsePacket request = new ChallengeResponsePacket();
        request.payload.result = clientSalt ^ serverSalt;
        SendToServer(request);
    }

    private void SendConnect() {
        //agregar cliente
    }

    private void ReceiveNetworkPacket<P>(NetworkPacket<P> packet) {
        switch (packet.packetType) {
            case PacketType.ConnectionRequest: {

            } break;
        }
    }
}