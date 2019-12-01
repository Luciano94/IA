﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public abstract class ReliableOrderPacket<P> : GamePacket<P> {
    private static Dictionary<uint, P> pendingPackets = new Dictionary<uint, P>();
    private uint idReceived = 0;
    private static uint lastIdExecuted = 0;
    private static uint lastIdSent = 0;

    public ReliableOrderPacket(PacketType packetType) : base(packetType, true) {
    }

    public void OnFinishDeserializing(Action<P> action) {
        uint nextId = lastIdExecuted + 1;
        if (idReceived == nextId) {
            action?.Invoke(payload);
            pendingPackets.Remove(idReceived);
            uint pendingID = idReceived + 1;
            while (pendingPackets.ContainsKey(pendingID)) {
                action?.Invoke(pendingPackets[pendingID]);
                pendingPackets.Remove(pendingID);
                pendingID++;
            }
            lastIdExecuted = pendingID - 1;
        } else if (idReceived > nextId) {
            pendingPackets.Add(idReceived, payload);
            if (pendingPackets.Count > 8) {
                List<uint> ids = pendingPackets.Keys.ToList();
                ids.Sort();
                uint pendingID = ids[0];
                while (pendingPackets.ContainsKey(pendingID)) {
                    action?.Invoke(pendingPackets[pendingID]);
                    pendingPackets.Remove(pendingID);
                    pendingID++;
                }
                lastIdExecuted = pendingID - 1;
            }
        }
    }

    public override void OnDeserialize(Stream stream) {
        BinaryReader binaryWriter = new BinaryReader(stream);
        idReceived = binaryWriter.ReadUInt32();
    }

    public override void OnSerialize(Stream stream) {
        BinaryWriter binaryWriter = new BinaryWriter(stream);
        binaryWriter.Write(++lastIdSent);
    }
}