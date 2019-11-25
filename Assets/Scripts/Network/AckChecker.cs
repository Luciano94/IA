using System.Collections.Generic;
using UnityEngine;

/*
Cuando me llegan paquetes reliable encolo los Ids, y cuando mando un paquete cualquiera
agarro el id mas grande y genero el array de bits. El ack es unico para cada cliente.
Los ids de orden son aparte y solo hay que tener en cuenta cuando estan cerca de INT_MAX
*/

public class AckChecker {
    public uint currAck = 0;
    public uint prevAckBitmask = 0;
    Dictionary<uint, byte[]> pendingPackets = new Dictionary<uint, byte[]>();

    public void QueuePacket(byte[] packet) {
        packet = PacketManager.Instance.WrapReliabilityOntoPacket(packet, true, currAck);
        pendingPackets.Add(currAck, packet);
        currAck++;
    }

    private void RemovePacket(uint id) {
        pendingPackets.Remove(id);
    }

    public void SendPendingPackets() {
        for (uint i = 0; i < pendingPackets.Count; ++i) {
            PacketManager.Instance.SendPacket(pendingPackets[i]);
        }
    }

    public void Write(uint packetId) {
        int diff = (int)((long)packetId - (long)currAck);
        if (diff > 0) {
            prevAckBitmask = prevAckBitmask << diff;
            prevAckBitmask |=  1U << (diff - 1);
        } else if (diff > -32) {
            prevAckBitmask |=  1U << (-diff);
        }
        currAck = packetId;
    }

    public bool Read(uint id) {
        int diff = (int)(currAck - id);
        if (diff == 0) {
            return true;
        } else if (diff < 0 || diff > 32) {
            return false;
        } else {
            return (prevAckBitmask & (1 << diff)) != 0;
        }
    }
}
