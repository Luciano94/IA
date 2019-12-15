using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerUDP : UnreliableOrderPacket<float[]>
{
    private uint OwnerPlayerID = 1;

    private bool needInterpolate = false;
    private float nextPosition = 0.0f;
    private Vector2 boundariesVector;
    private float lastInput;
    private float timeDiff;
    public float floor = -3.5f;
    public float roof = 3.5f;
    private float playerSpeed;

    private const float timeToReconciliate = 5.0f;
    private float reconciliateTimer = 0f;

    private void Start() {
        nextPosition = transform.position.y;
        playerSpeed = PongManager.Instance.playerSpeed;
        boundariesVector = new Vector2(transform.position.x, 0);
    }

    private void Update() {
        if (ConnectionManager.Instance.isServer) {
            reconciliateTimer += Time.unscaledDeltaTime;
            if (reconciliateTimer > timeToReconciliate) {
                float[] data = new float[3];
                data[0] = transform.position.x;
                data[1] = transform.position.y;
                data[2] = PongManager.Instance.GetTime() - timeDiff;
                MessageManager.Instance.SendTimedPosition(data, OwnerPlayerID);
            }
        }
    }

    private void FixedUpdate() {
        if(lastInput != 0)
        {
            transform.Translate(0,lastInput * playerSpeed * Time.fixedDeltaTime,0);
        }
        
        CheckBounduaries();
    }


    private void CheckBounduaries()
    {
        if(transform.position.y > roof)
        {
            boundariesVector.y = roof;
            transform.position = boundariesVector;
        }
        else if(transform.position.y < floor)
        {
            boundariesVector.y = floor;
            transform.position = boundariesVector;
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
            case (ushort)UserPacketType.PlayerInput:
                PlayerInputPacket playerInput = new PlayerInputPacket();
                idReceived = playerInput.Deserialize(stream);
                OnFinishDeserializing(SetPlayerPosition, playerInput.payload);
                //SetPlayerPosition(playerInput.payload);
            break;
        }
    }

    private void SetPlayerPosition(float[] playerInput) {
        timeDiff = Mathf.Abs(PongManager.Instance.GetTime() - playerInput[1]);
        lastInput = playerInput[0];
    }
}
