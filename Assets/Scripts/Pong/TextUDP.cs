﻿using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class TextUDP : MonoBehaviour {
    public Text text;
    public uint objectID;

    public void AddListener() {
        PacketManager.Instance.AddListener(objectID, OnReceivePacket);
    }

    public void SetText(string text) {
        this.text.text = text;
        MessageManager.Instance.SendString(text, objectID);
    }

    
    void OnReceivePacket(uint packetId, ushort type, Stream stream)
    {
        switch (type)
        {
            case (ushort)UserPacketType.Message:
                MessagePacket messagePacket = new MessagePacket();
                messagePacket.Deserialize(stream);
                text.text = messagePacket.payload;
            break;
        }
    }
}
