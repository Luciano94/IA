using System.Collections.Generic;
using UnityEngine;

public class AckChecker : MonoBehaviour {
    public uint currAck = 0;
    public uint prevAckBitmask = 0;
    List<ISerializePacket> pendingPackets;

    void Write(uint packetId) {
        int diff = (int)((long)packetId - (long)currAck);
        if (diff > 0) {
            prevAckBitmask = prevAckBitmask << diff;
            prevAckBitmask |=  1U << (diff - 1);
        } else if (diff > -32) {
            prevAckBitmask |=  1U << (-diff);
        }
        currAck = packetId;
    }

    bool Read(uint id) {
        int diff = currAck - id;
        if (diff == 0) {
            return true;
        } else if (diff < 0 || diff > 32) {
            return false;
        } else {
            return (prevAckBitmask & (1 << diff)) != 0;
        }
    }
}
