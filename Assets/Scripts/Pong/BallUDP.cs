using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BallUDP : MonoBehaviour
{
    private uint OwnerBallID = 2;

    private bool needInterpolate = false;
    private Vector3 nextPosition;
    private float ballSpeed;

    private void Start() {
        nextPosition = new Vector3(0,0,0);
        ballSpeed = PongManager.Instance.ballSpeed;
    }

    private void Update() {
        if(needInterpolate){
            transform.position = Vector3.Lerp(transform.position, nextPosition, Time.deltaTime * ballSpeed);
            if(transform.position == nextPosition)
                needInterpolate = false;
        }
    }

    void OnEnable()
    {
        PacketManager.Instance.AddListener(OwnerBallID, OnReceivePacket);
    }

    void OnDisable()
    {
        PacketManager.Instance.RemoveListener(OwnerBallID);
    }
    
    void SetBallPosition(float[] ballPacket){
        float timeDiff = Mathf.Abs( PongManager.Instance.GetTime() - ballPacket[3]);
        if(timeDiff != 0){
            nextPosition = new Vector3(ballPacket[0], ballPacket[1], ballPacket[2]);
            needInterpolate = true;
        }
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
            case (ushort)UserPacketType.BallInput:
                BallInputPacket ballPacket = new BallInputPacket();
                ballPacket.Deserialize(stream);
                ballPacket.OnFinishDeserializing(SetBallPosition);
            break;
        }
    }

    private void Move(Vector3 position) {
        transform.position = position;
    }
}
