using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public abstract class UnreliableOrderPacket<P> : GamePacket<P> {
    private uint idReceived = 0;
    private static uint lastIdExecuted = 0;
    private static uint lastIdSent = 0;

    public UnreliableOrderPacket(PacketType packetType) : base(packetType, false) {
    }

    public void OnFinishDeserializing(Action<P> action) {
        if (idReceived > lastIdExecuted) {
            action?.Invoke(payload);
            lastIdExecuted = idReceived;
        }  else if (idReceived < lastIdExecuted) {
            UnityEngine.Debug.Log("rechazado");
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
