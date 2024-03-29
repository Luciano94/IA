﻿using UnityEngine;

public enum GameState
{
    GameStart,
    GameEnd,
    PlayerOneWin,
    PlayerTwoWin,
    Count
}

public struct GameStatePayload
{
    public GameState gState;
}

public class MessageManager : Singleton<MessageManager>
{
    protected override void Initialize()
    {
        base.Initialize();
    }

    public void SendString(string message, uint objectId)
    {
        MessagePacket packet = new MessagePacket();

        packet.payload = message;
        PacketManager.Instance.SendPacket(packet, objectId, false);
    }

    public void SendPosition(Vector3 position, uint objectId)
    {
        PositionPacket packet = new PositionPacket();


        packet.payload = position;

        PacketManager.Instance.SendPacket(packet, objectId, packet.reliable);
    }

    public void SendTimedPosition(float[] position, uint objectId)
    {
        TimedPositionPacket packet = new TimedPositionPacket();

        packet.payload = position;

        PacketManager.Instance.SendPacket(packet, objectId, packet.reliable);
    }

    public void SendBallPosition(float[] ballPosition, uint objectId, uint id)
    {
        BallInputPacket packet = new BallInputPacket();

        packet.payload = ballPosition;

        PacketManager.Instance.SendPacket(packet, objectId, packet.reliable, id);
    }

    public void SendInt(int number, uint objectId)
    {
        IntPacket packet = new IntPacket();

        packet.payload = number;

        PacketManager.Instance.SendPacket(packet, objectId, false);
    }

    public void SendPlayerInput(float[] playerInput, uint objectId, uint id)
    {
        PlayerInputPacket packet = new PlayerInputPacket();

        packet.payload = playerInput;

        PacketManager.Instance.SendPacket(packet, objectId, packet.reliable, id);
    }


    public void SendGameState(GameState gState, uint objectId)
    {

        GameStatePacket packet = new GameStatePacket();

        packet.payload = gState;

        PacketManager.Instance.SendPacket(packet, objectId, false);
    }
}