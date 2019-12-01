using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerUDP : MonoBehaviour
{
    private uint OwnerPlayerID = 1;

    void OnEnable()
    {
        PacketManager.Instance.AddListener(OwnerPlayerID, OnReceivePacket);
    }

    void OnDisable()
    {
        PacketManager.Instance.RemoveListener(OwnerPlayerID);
    }

    void OnReceivePacket(ushort type, Stream stream)
    {
        switch (type)
        {
            case (ushort)UserPacketType.Position:
                PositionPacket positionPacket = new PositionPacket();
                positionPacket.Deserialize(stream);
                positionPacket.OnFinishDeserializing(Move);
            break;
        }
    }

    private void Move(Vector3 position) {
        transform.position = position;
    }
}
