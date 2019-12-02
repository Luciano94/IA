using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BallUDP : ReliableOrderPacket<float[]>
{
    private uint OwnerBallID = 2;

    private bool needInterpolate = false;
    private Vector3 nextPosition;
    private float ballSpeed;

    private float minReconcilationDistance = 5.0f;


    private void Start() {
        nextPosition = new Vector3(0,0,0);
        ballSpeed = PongManager.Instance.ballSpeed;
    }

    private void FixedUpdate() {
        if(needInterpolate){
            transform.position = Vector3.Lerp(transform.position, nextPosition, Time.fixedDeltaTime);
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
        float timeDiff = PongManager.Instance.GetTime() - ballPacket[2];
        nextPosition = transform.position;
        nextPosition += new Vector3(ballPacket[0], ballPacket[1], 0) * timeDiff;
        needInterpolate = true;
    }

    void OnReceivePacket(ushort type, Stream stream)
    {
        switch (type)
        {
            case (ushort)UserPacketType.BallInput:
                BallInputPacket ballPacket = new BallInputPacket();
                idReceived = ballPacket.Deserialize(stream);
                OnFinishDeserializing(SetBallPosition, ballPacket.payload);
                //SetBallPosition(ballPacket.payload);
            break;
        }
    }
}
