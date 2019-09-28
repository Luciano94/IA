public interface IPacket {
    byte[] Serialize();

    void Deserialize(byte[] data);
}
