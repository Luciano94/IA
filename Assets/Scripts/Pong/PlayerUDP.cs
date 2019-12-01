using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerUDP : MonoBehaviour
{
    private uint OwnerPlayerID = 1;

    private bool needInterpolate = false;
    private float nextPosition = 0.0f;
    private float playerSpeed;

    private void Start() {
        nextPosition = transform.position.y;
        playerSpeed = PongManager.Instance.playerSpeed;
    }

    private void Update() {
        if(needInterpolate){
            Vector3 newPos = new Vector3(transform.position.x,nextPosition, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * playerSpeed);
            if(transform.position.y == nextPosition)
                needInterpolate = false;
        }
    }
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
            case (ushort)UserPacketType.PlayerInput:
                PlayerInputPacket playerInput = new PlayerInputPacket();
                playerInput.Deserialize(stream);
                playerInput.OnFinishDeserializing(SetPlayerPosition);
            break;
        }
    }

    private void SetPlayerPosition(float[] playerInput){
        float timeDiff = Mathf.Abs( PongManager.Instance.GetTime() - playerInput[1]);
        if(playerInput[0] != 0){
            nextPosition = playerInput[0];
            needInterpolate = true;
        }
    }

    private void Move(Vector3 position) {
        transform.position = position;
    }
}
