using System;
using System.IO;
using System.Collections.Generic;

public abstract class ReliableOrderPacket<P> : GamePacket<P> {
    private Dictionary<uint, P> pendingPackets = new Dictionary<uint, P>();
    private static uint lastIdReceived = 0;
    private static uint lastIdExecuted = 0;
    private static uint lastIdSent = 0;

    public ReliableOrderPacket(PacketType packetType) : base(packetType, true) {
    }

    public void OnFinishDeserializing(Action<P> action) {
        uint nextId = lastIdExecuted + 1;
        if (lastIdReceived == nextId) {
            action?.Invoke(payload);
            uint pendingID = lastIdReceived + 1;
            while (pendingPackets.ContainsKey(pendingID)) {
                action?.Invoke(pendingPackets[pendingID]);
                pendingPackets.Remove(pendingID);
                pendingID++;
            }
            lastIdExecuted = pendingID - 1;
        } else if (lastIdReceived > nextId) {
            pendingPackets.Add(lastIdReceived, payload);
        }
    }

    public override void OnDeserialize(Stream stream) {
        BinaryReader binaryWriter = new BinaryReader(stream);
        lastIdReceived = binaryWriter.ReadUInt32();
    }

    public override void OnSerialize(Stream stream) {
        BinaryWriter binaryWriter = new BinaryWriter(stream);
        binaryWriter.Write(++lastIdSent);
    }
}
