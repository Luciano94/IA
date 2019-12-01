using System.IO;
using UnityEngine;

public abstract class GamePacket<P> : NetworkPacket<P> {
    public readonly bool reliable;
    public GamePacket(PacketType packetType, bool reliable = false) : base(packetType) {
        this.reliable = reliable;
    }
}

public class MessagePacket : GamePacket<string> {
    public MessagePacket() : base(global::PacketType.User, true) {
        userPacketType = (ushort)UserPacketType.Message;
    }

    public override void OnSerialize(Stream stream) {
        BinaryWriter binaryWriter = new BinaryWriter(stream);
        binaryWriter.Write(payload);
    }

    public override void OnDeserialize(Stream stream) {
        BinaryReader binaryReader = new BinaryReader(stream);
        payload = binaryReader.ReadString();
    }
}

public class PositionPacket : ReliableOrderPacket<Vector3> {
    public PositionPacket() : base(global::PacketType.User) {
        userPacketType = (ushort)UserPacketType.Position;
    }

    public override void OnSerialize(Stream stream) {
        base.OnSerialize(stream);
        BinaryWriter binaryWriter = new BinaryWriter(stream);
        binaryWriter.Write(payload.x);
        binaryWriter.Write(payload.y);
        binaryWriter.Write(payload.z);
    }

    public override void OnDeserialize(Stream stream) {
        base.OnDeserialize(stream);
        BinaryReader binaryReader = new BinaryReader(stream);
        payload.x = binaryReader.ReadSingle();
        payload.y = binaryReader.ReadSingle();
        payload.z = binaryReader.ReadSingle();
    }
}

public class IntPacket : GamePacket<int> {
    public IntPacket() : base(global::PacketType.User) {
        userPacketType = (ushort)UserPacketType.Int;
    }

    public override void OnSerialize(Stream stream) {
        BinaryWriter binaryWriter = new BinaryWriter(stream);
        binaryWriter.Write(payload);
    }

    public override void OnDeserialize(Stream stream) {
        BinaryReader binaryReader = new BinaryReader(stream);
        payload = binaryReader.ReadInt32();
    }
}

public class GameStatePacket : GamePacket<GameState> {
    public GameStatePacket() : base(global::PacketType.User, true) {
        userPacketType = (ushort)UserPacketType.GameState;
    }

    public override void OnSerialize(Stream stream) {
        BinaryWriter binaryWriter = new BinaryWriter(stream);
        binaryWriter.Write((ushort)payload);
    }

    public override void OnDeserialize(Stream stream) {
        BinaryReader binaryReader = new BinaryReader(stream);
        payload = (GameState)binaryReader.ReadUInt16();
    }
}