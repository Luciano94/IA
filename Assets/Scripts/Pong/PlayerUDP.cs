using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerUDP : MonoBehaviour
{
    void OnEnable()
    {
        PacketManager.Instance.AddListener(1, OnReceivePacket);
    }

    void OnDisable()
    {
        PacketManager.Instance.RemoveListener(1);
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
               // Debug.Log("[PAYLOAD]= " + positionPacket.payload);
                transform.position = positionPacket.payload;

                // Aca enviaria el payload a donde sea necesario
            break;
        }
    }
}
