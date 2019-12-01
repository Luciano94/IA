using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BallUDP : MonoBehaviour
{
    private uint OwnerBallID = 2;

    void OnEnable()
    {
        PacketManager.Instance.AddListener(OwnerBallID, OnReceivePacket);
    }

    void OnDisable()
    {
        PacketManager.Instance.RemoveListener(OwnerBallID);
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
