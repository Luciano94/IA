using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using UnityEngine;

public class PacketManager : MBSingleton<PacketManager>, IReceiveData {
    private Dictionary<uint, System.Action<uint, ushort, Stream>> onPacketReceived = new Dictionary<uint, System.Action<uint, ushort, Stream>>();
    private uint currentPacketId = 0;

    protected override void Awake() {
        base.Awake();
        NetworkManager.Instance.OnReceiveEvent += OnReceiveData;
    }

    public void AddListener(uint ownerId, System.Action<uint, ushort, Stream> callback) {
        if (!onPacketReceived.ContainsKey(ownerId)) {
            onPacketReceived.Add(ownerId, callback);
        }
    }

    public void RemoveListener(uint ownerId) {
        if (onPacketReceived.ContainsKey(ownerId)) {
            onPacketReceived.Remove(ownerId);
        }
    }

    public byte[] WrapCheckSumOntoPacket(byte[] packet) {
        MemoryStream stream = new MemoryStream();
        BinaryWriter binaryWriter = new BinaryWriter(stream);
        
        
        using (MD5 md5Hash = MD5.Create()) {
            byte[] hash = md5Hash.ComputeHash(packet);
            MemoryStream streamHash = new MemoryStream(hash);
            BinaryReader hashReader = new BinaryReader(streamHash);
            int hash32 = hashReader.ReadInt32();
            binaryWriter.Write(hash32);
        }
        stream.Close();
        byte[] checkSum = stream.ToArray();
        byte[] wrappedBytes = new byte[checkSum.Length + packet.Length];
        checkSum.CopyTo(wrappedBytes, 0);
        packet.CopyTo(wrappedBytes, checkSum.Length);

        return wrappedBytes;
    }

    public byte[] WrapReliabilityOntoPacket(byte[] packet, bool reliable, uint ackID = 0) {
        MemoryStream stream = new MemoryStream();
        BinaryWriter binaryWriter = new BinaryWriter(stream);
        binaryWriter.Write(reliable);
        if (reliable) {
            binaryWriter.Write(ackID);
        }
        stream.Close();
        byte[] reliability = stream.ToArray();
        byte[] wrappedBytes = new byte[reliability.Length + packet.Length];
        reliability.CopyTo(wrappedBytes, 0);
        packet.CopyTo(wrappedBytes, reliability.Length);

        wrappedBytes = WrapCheckSumOntoPacket(wrappedBytes);

        return wrappedBytes;
    }

    public void SendPacket<T>(NetworkPacket<T> packet, uint objectId, bool reliable = false) {
        byte[] bytes = Serialize(packet, objectId);
        
        if (reliable) {
            ConnectionManager.Instance.QueuePacket(bytes);
        } else {
            bytes = WrapReliabilityOntoPacket(bytes, false);
            if (ConnectionManager.Instance.isServer) {
                NetworkManager.Instance.Broadcast(bytes);
            } else {
                NetworkManager.Instance.SendToServer(bytes);
            }
        }
    }

    public void SendPacket<T>(NetworkPacket<T> packet) {
        byte[] bytes = Serialize(packet);
        bytes = WrapReliabilityOntoPacket(bytes, false);

        if (ConnectionManager.Instance.isServer) {
            NetworkManager.Instance.Broadcast(bytes);
        } else {
            NetworkManager.Instance.SendToServer(bytes);
        }
    }

    public void SendPacket<T>(NetworkPacket<T> packet, IPEndPoint ipEndPoint) {
        byte[] bytes = Serialize(packet);
        bytes = WrapReliabilityOntoPacket(bytes, false);
        
        NetworkManager.Instance.SendToClient(bytes, ipEndPoint);
    }

    public void SendPacket(byte[] packet) {
        if (ConnectionManager.Instance.isServer) {
            NetworkManager.Instance.Broadcast(packet);
        } else {
            NetworkManager.Instance.SendToServer(packet);
        }
    }

    byte[] Serialize<T>(NetworkPacket<T> packet) {
        PacketHeader header = new PacketHeader();
        MemoryStream stream = new MemoryStream();

        header.protocolId = NetworkManager.PROTOCOL_ID;
        header.packetType = packet.packetType;

        header.Serialize(stream);
        packet.Serialize(stream);

        stream.Close();

        return stream.ToArray();
    }

    byte[] Serialize<T>(NetworkPacket<T> packet, uint objectId) {
        PacketHeader header = new PacketHeader();
        UserPacketHeader userHeader = new UserPacketHeader();
        MemoryStream stream = new MemoryStream();

        header.protocolId = NetworkManager.PROTOCOL_ID;

        header.packetType = packet.packetType;

        if (packet.packetType == PacketType.User) {
            userHeader.packetType = packet.userPacketType;
            userHeader.packetId = currentPacketId++;
            userHeader.senderId = NetworkManager.Instance.clientId;
            userHeader.objectId = objectId;
        }

        header.Serialize(stream);
        userHeader.Serialize(stream);
        packet.Serialize(stream);

        stream.Close();

        return stream.ToArray();
    }

    public void OnReceiveData(byte[] data, IPEndPoint ipEndpoint) {
        PacketHeader header = new PacketHeader();
        MemoryStream stream = new MemoryStream(data);

        BinaryReader binaryReader = new BinaryReader(stream);

        int hash32 = binaryReader.ReadInt32();

        byte[] dataWithoutHash = new byte[data.Length - 4];
        Array.Copy(data, 4, dataWithoutHash, 0, data.Length - 4);

#if IA_DEBUG
        if (UnityEngine.Random.Range(0f, 100f) < 0.5f) {
            for (int i = 0; i < dataWithoutHash.Length; i++) {
                if (dataWithoutHash[i] != 0) {
                    dataWithoutHash[i] = 0;
                    break;
                }
            }
        }
#endif
        int ourHash;
        using (MD5 md5Hash = MD5.Create()) {
            byte[] hash = md5Hash.ComputeHash(dataWithoutHash);
            MemoryStream hashStream = new MemoryStream(hash);
            BinaryReader hashReader = new BinaryReader(hashStream);
            ourHash = hashReader.ReadInt32();
            hashStream.Close();
        }

        if (hash32 == ourHash) {
            bool reliability = binaryReader.ReadBoolean();

            if (reliability) {
                uint ack = binaryReader.ReadUInt32();
            }

            header.Deserialize(stream);
        
            if (header.packetType == PacketType.User) {
                while (stream.Length - stream.Position > 0) {
                    UserPacketHeader userHeader = new UserPacketHeader();
                    userHeader.Deserialize(stream);
                    InvokeCallback(userHeader.objectId, userHeader.packetId, userHeader.packetType, stream);
                }
            } else {
                ConnectionManager.Instance.OnReceivePacket(ipEndpoint, header.packetType, stream);
            }
        } else {
            Debug.LogWarning("PACKAGE CORRUPTED");
        }

        stream.Close();
    }

    void InvokeCallback(uint objectId, uint packetId, ushort packetType, Stream stream) {
        if (onPacketReceived.ContainsKey(objectId)) {
            onPacketReceived[objectId].Invoke(packetId, packetType, stream);
        }
    }
}