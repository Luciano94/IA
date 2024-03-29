using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using UnityEngine;

public class PacketManager : MBSingleton<PacketManager>, IReceiveData {
    private Dictionary<uint, System.Action<ushort, Stream>> onPacketReceived = new Dictionary<uint, System.Action<ushort, Stream>>();

    protected override void Awake() {
        base.Awake();
        NetworkManager.Instance.OnReceiveEvent += OnReceiveData;
    }

    public void AddListener(uint ownerId, System.Action<ushort, Stream> callback) {
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
            int hash32 = hash[0];
            for (int i = 1, shift = 8; i < 4; i++, shift += 8) {
                hash32 += hash[i] << shift;
            }
            binaryWriter.Write(hash32);
        }
        stream.Close();
        byte[] checkSum = stream.ToArray();
        byte[] wrappedBytes = new byte[checkSum.Length + packet.Length];
        checkSum.CopyTo(wrappedBytes, 0);
        packet.CopyTo(wrappedBytes, checkSum.Length);

        return wrappedBytes;
    }

    public byte[] WrapReliabilityOntoPacket(byte[] packet, bool reliable, AckChecker ackChecker, uint ackId = 0) {
        MemoryStream stream = new MemoryStream();
        BinaryWriter binaryWriter = new BinaryWriter(stream);
        binaryWriter.Write(reliable);
        if (reliable) {
            binaryWriter.Write(ackId);
            uint lastAck, ackArray;
            bool hasToConfirm = ackChecker.GetAckConfirmation(out lastAck, out ackArray);
            binaryWriter.Write(hasToConfirm);
            if (hasToConfirm) {
                binaryWriter.Write(lastAck);
                binaryWriter.Write(ackArray);
            }
        }
        stream.Close();
        byte[] reliability = stream.ToArray();
        byte[] wrappedBytes = new byte[reliability.Length + packet.Length];
        reliability.CopyTo(wrappedBytes, 0);
        packet.CopyTo(wrappedBytes, reliability.Length);

        wrappedBytes = WrapCheckSumOntoPacket(wrappedBytes);

        return wrappedBytes;
    }

    public void SendPacket<T>(NetworkPacket<T> packet, uint objectId, bool reliable) {
        byte[] bytes = Serialize(packet, objectId);
        
        if (ConnectionManager.Instance.isServer) {
            NetworkManager.Instance.Broadcast(bytes, reliable);
        } else {
            AckChecker ackChecker = ConnectionManager.Instance.OwnClient.ackChecker;
            uint ackId = ackChecker.NewAck;
            bytes = WrapReliabilityOntoPacket(bytes, reliable, ackChecker, ackId);
            NetworkManager.Instance.SendToServer(bytes);
            if (reliable) {
                ConnectionManager.Instance.QueuePacket(bytes, ackId);
            }
        }
    }

    public void SendPacket<T>(OrderedNetworkPacket<T> packet, uint objectId, bool reliable, uint id) {
        byte[] bytes = Serialize(packet, objectId, id);
        
        if (ConnectionManager.Instance.isServer) {
            NetworkManager.Instance.Broadcast(bytes, reliable);
        } else {
            AckChecker ackChecker = ConnectionManager.Instance.OwnClient.ackChecker;
            uint ackId = ackChecker.NewAck;
            bytes = WrapReliabilityOntoPacket(bytes, reliable, ackChecker, ackId);
            NetworkManager.Instance.SendToServer(bytes);
            if (reliable) {
                ConnectionManager.Instance.QueuePacket(bytes, ackId);
            }
        }
    }

    public void SendPacket<T>(NetworkPacket<T> packet) {
        byte[] bytes = Serialize(packet);

        if (ConnectionManager.Instance.isServer) {
            NetworkManager.Instance.Broadcast(bytes, false);
        } else {
            bytes = WrapReliabilityOntoPacket(bytes, false, ConnectionManager.Instance.OwnClient.ackChecker);
            NetworkManager.Instance.SendToServer(bytes);
        }
    }

    public void SendPacket<T>(NetworkPacket<T> packet, IPEndPoint ipEndPoint) {
        byte[] bytes = Serialize(packet);
        bytes = WrapReliabilityOntoPacket(bytes, false, ConnectionManager.Instance.clients[ConnectionManager.Instance.ipToId[ipEndPoint]].ackChecker);
        
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

        header.protocolId = NetworkManager.PROTOCOL_ID; // TODO

        header.packetType = packet.packetType;

        if (packet.packetType == PacketType.User) {
            userHeader.packetType = packet.userPacketType;
            userHeader.senderId = NetworkManager.Instance.clientId;
            userHeader.objectId = objectId;
        }

        header.Serialize(stream);
        userHeader.Serialize(stream);
        packet.Serialize(stream);

        stream.Close();

        return stream.ToArray();
    }

    byte[] Serialize<T>(OrderedNetworkPacket<T> packet, uint objectId, uint id) {
        PacketHeader header = new PacketHeader();
        UserPacketHeader userHeader = new UserPacketHeader();
        MemoryStream stream = new MemoryStream();

        header.protocolId = NetworkManager.PROTOCOL_ID; // TODO

        header.packetType = packet.packetType;

        if (packet.packetType == PacketType.User) {
            userHeader.packetType = packet.userPacketType;
            userHeader.senderId = NetworkManager.Instance.clientId;
            userHeader.objectId = objectId;
        }

        header.Serialize(stream);
        userHeader.Serialize(stream);
        packet.Serialize(stream, id);

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

#if DEBUG_CHECKSUM
        if (UnityEngine.Random.Range(0f, 100f) < 0.5f) {
            if (dataWithoutHash[0] != 0) {
                dataWithoutHash[0] = 0;
            } else {
                dataWithoutHash[0] = 1;
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
#if DEBUG_RELIABLE
                if (UnityEngine.Random.Range(0f, 100f) < 0.6f) {
                // if (Input.GetKey(KeyCode.A)) {
                    stream.Close();
                    return;
                }
#endif
                uint packageAck = binaryReader.ReadUInt32();
                if (ConnectionManager.Instance.isServer) {
                    Client client = ConnectionManager.Instance.clients[ConnectionManager.Instance.ipToId[ipEndpoint]];
                    client.ackChecker.RegisterPackageReceived(packageAck);
                } else {
                    ConnectionManager.Instance.OwnClient.ackChecker.RegisterPackageReceived(packageAck);
                }
                
                bool hasAck = binaryReader.ReadBoolean();
                if (hasAck) {
                    uint lastAck = binaryReader.ReadUInt32();
                    uint prevAckArray = binaryReader.ReadUInt32();
                    
                    if (ConnectionManager.Instance.isServer) {
                        Client client = ConnectionManager.Instance.clients[ConnectionManager.Instance.ipToId[ipEndpoint]];
                        client.ackChecker.ClearPackets(lastAck, prevAckArray);
                    } else {
                        ConnectionManager.Instance.OwnClient.ackChecker.ClearPackets(lastAck, prevAckArray);
                    }
                }
            }

            header.Deserialize(stream);
        
            if (header.packetType == PacketType.User) {
                while (stream.Length - stream.Position > 0) {
                    UserPacketHeader userHeader = new UserPacketHeader();
                    userHeader.Deserialize(stream);
                    InvokeCallback(userHeader.objectId, userHeader.packetType, stream);
                }
            } else {
                ConnectionManager.Instance.OnReceivePacket(ipEndpoint, header.packetType, stream);
            }
        } else {
            Debug.LogWarning("PACKAGE CORRUPTED");
        }

        stream.Close();
    }

    void InvokeCallback(uint objectId, ushort packetType, Stream stream) {
        if (onPacketReceived.ContainsKey(objectId)) {
            onPacketReceived[objectId].Invoke(packetType, stream);
        }
    }
}