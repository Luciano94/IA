using System.Collections.Generic;
using UnityEngine;

/*
Cuando me llegan paquetes reliable encolo los Ids, y cuando mando un paquete cualquiera
agarro el id mas grande y genero el array de bits. El ack es unico para cada cliente.
Los ids de orden son aparte y solo hay que tener en cuenta cuando estan cerca de INT_MAX
*/

public class AckChecker {
    private uint currAck = 0;
    public uint NewAck { get { return currAck++; } }
    Dictionary<uint, byte[]> pendingPackets = new Dictionary<uint, byte[]>();
    List<uint> packetsReceived = new List<uint>();

    public void QueuePacket(byte[] packet, uint id) {
        pendingPackets.Add(id, packet);

        if (id > 32) {
            List<uint> toDelete = new List<uint>();
            uint oldID = id - 32;
            Dictionary<uint, byte[]>.Enumerator iterator = pendingPackets.GetEnumerator();
            while (iterator.MoveNext()) {
                if (iterator.Current.Key < oldID) {
                    toDelete.Add(iterator.Current.Key);
                }
            }

            int toDeleteSize = toDelete.Count;
            for (int i = 0; i < toDeleteSize; i++) {
                pendingPackets.Remove(toDelete[i]);
            }
        }
    }

    private void RemovePacket(uint id) {
        pendingPackets.Remove(id);
    }

    public void SendPendingPackets() {
        Dictionary<uint, byte[]>.Enumerator iterator = pendingPackets.GetEnumerator();
        while (iterator.MoveNext()) {
            PacketManager.Instance.SendPacket(iterator.Current.Value);
        }
    }

    public void Write(uint id, ref uint lastAck, ref uint ackArray) {
        int diff = (int)((long)id - (long)lastAck);
        if (diff > 0) {
            ackArray = ackArray << diff;
            ackArray |=  1U << (diff - 1);
            lastAck = id;
        } else if (diff > -32) {
            ackArray |=  1U << (-diff);
        }
    }

    public bool Read(uint id, uint lastAck, uint ackArray) {
        int diff = (int)(lastAck - id);
        if (diff == 0) {
            return true;
        } else if (diff < 0 || diff > 32) {
            return false;
        } else {
            return (ackArray & (1 << diff)) != 0;
        }
    }

    public void RegisterPackageReceived(uint id) {
        packetsReceived.Add(id);

        if (id > 32) {
            List<uint> toDelete = new List<uint>();
            uint oldID = id - 32;
            int packetsReceivedSize = packetsReceived.Count;
            for (int i = 0; i < packetsReceivedSize; i++) {
                if (packetsReceived[i] < oldID) {
                    toDelete.Add(packetsReceived[i]);
                }
            }
            int toDeleteSize = toDelete.Count;
            for (int i = 0; i < toDeleteSize; i++) {
                packetsReceived.Remove(toDelete[i]);
            }
        }
    }
    
    public bool GetAckConfirmation(out uint lastAck, out uint ackArray) {
        lastAck = 0;
        ackArray = 0;
        int receivedCount = packetsReceived.Count;
        if (receivedCount == 0) {
            return false;
        }

        for (int i = 0; i < receivedCount; i++) {
            Write(packetsReceived[i], ref lastAck, ref ackArray);
        }

        return true;
    }

    public void ClearPackets(uint lastAck, uint prevAckArray) {
        pendingPackets.Remove(lastAck);
        uint id = lastAck - 1;
        for (uint i = 0; i < 32; i++, id--) {
            if (Read(id, lastAck, prevAckArray)) {
                pendingPackets.Remove(id);
            }
        }
    }
}
