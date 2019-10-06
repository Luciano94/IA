using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BallUDP : MonoBehaviour
{
    void OnEnable()
    {
        PacketManager.Instance.Awake();
        PacketManager.Instance.AddListener(2, OnReceivePacket);
    }

    void OnDisable()
    {
        PacketManager.Instance.RemoveListener(2);
    }

    void OnReceivePacket(uint packetId, ushort type, Stream stream)
    {
        switch (type)
        {
            case (ushort)UserPacketType.Message:
                MessagePacket messagePacket = new MessagePacket();
                messagePacket.Deserialize(stream);
            break;
            case (ushort)UserPacketType.Position:
                PositionPacket positionPacket = new PositionPacket();
                positionPacket.Deserialize(stream);
                transform.position = positionPacket.payload;

                // Aca enviaria el payload a donde sea necesario
            break;
        }
    }
}
