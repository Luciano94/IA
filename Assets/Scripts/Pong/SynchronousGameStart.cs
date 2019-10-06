using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SynchronousGameStart : MonoBehaviour
{
    public uint objectID;

    public void AddListener()
    {
        SendState(GameState.Count);
        PacketManager.Instance.AddListener(objectID, OnReceivePacket);
    }

    public void SendState(GameState gState)
    {
        MessageManager.Instance.SendGameState(gState, objectID);
    }


    void OnReceivePacket(uint packetId, ushort type, Stream stream)
    {
        switch (type)
        {
            case (ushort)UserPacketType.GameState:
                GameStatePacket gStateMessage = new GameStatePacket();
                gStateMessage.Deserialize(stream);
                switch (gStateMessage.payload)
                {
                    case GameState.GameStart:
                        PongManager.Instance.StartGame();
                        break;
                }
                break;
        }
    }
}
