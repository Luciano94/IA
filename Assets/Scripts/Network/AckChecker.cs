using System.Collections.Generic;
using UnityEngine;

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
