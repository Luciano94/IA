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
    public float floor = -3.5f;
    public float roof = 3.5f;
    private float playerSpeed;

    //reconciliate vieja
    private float timeToReconciliate = 5.0f;
    private float actualTimeToReconciliate = 5.0f;



    private void Start() {
        nextPosition = transform.position.y;
        playerSpeed = PongManager.Instance.playerSpeed;
        boundariesVector = new Vector2(transform.position.x, 0);
    }

    private void FixedUpdate() {
        if(needInterpolate){
            Vector3 newPos = new Vector3(transform.position.x,nextPosition, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, newPos, Time.fixedDeltaTime);
            if(transform.position.y == nextPosition){
                needInterpolate = false;
            }
        }
        CheckBounduaries();
        //Reconciliate();
    }

    private void Reconciliate(){
        actualTimeToReconciliate += Time.fixedDeltaTime;
        if(actualTimeToReconciliate >= timeToReconciliate){
            actualTimeToReconciliate = 0.0f;
            SendInfo();
        } 
    }

    private void SendInfo()
    {
        float[] playerInput = new float[2];
        playerInput[0] = transform.position.y;
        playerInput[1] = PongManager.Instance.GetTime();
        MessageManager.Instance.SendPlayerInput(playerInput, OwnerPlayerID, ++lastIdSent);
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

    private void SetPlayerPosition(float[] playerInput){
            float timeDiff = Mathf.Abs(PongManager.Instance.GetTime() - playerInput[1]);
            nextPosition = transform.position.y + playerInput[0] * timeDiff;   
            needInterpolate = true;
    }
}
