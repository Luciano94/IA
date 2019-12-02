using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PlayerScript : UnreliableOrderPacket<float[]>
{
    private uint OwnerPlayerID = 1;

    public float verticalAxis;
    private Vector2 boundariesVector;
    public float speed = 10;
    public float floor = -3.5f;
    public float roof = 3.5f;

    private float playerInputValue = 0.0f;
    private float lastValue;
    public int packetsTicks = 10;
    public int actualTicks = 0;

    //Reconcilation
    List<Vector3> playerInputs;
    float maxDistanceBetweenPositions = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        speed = PongManager.Instance.playerSpeed;
        boundariesVector = new Vector2(transform.position.x, 0);
        playerInputValue = transform.position.y;
        lastValue = 0;
        playerInputs = new List<Vector3>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Movement();
        CheckBounduaries();
    }

    private void SendInfo()
    {
        playerInputs.Add(new Vector3(verticalAxis * speed, transform.position.y, PongManager.Instance.GetTime()));
        float[] playerInput = new float[2];
        playerInput[0] = verticalAxis * speed;
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

    private void Movement()
    {
        verticalAxis = Input.GetAxisRaw("Vertical");
        if(verticalAxis != 0)
        {
            transform.Translate(0,verticalAxis * speed * Time.fixedDeltaTime,0);
        }

        EvaluatePlayerInfo();
    }

    private void EvaluatePlayerInfo(){
        if(lastValue != verticalAxis){
            lastValue = verticalAxis;
            SendInfo();
        } 
    }

  /*  void OnReceivePacket(ushort type, Stream stream)
    {
        switch (type)
        {
            case (ushort)UserPacketType.PlayerInput:
                PlayerInputPacket playerInput = new PlayerInputPacket();
                playerInput.Deserialize(stream);
                PlayerPositionReconciliationChecker(playerInput.payload);
                //playerInput.OnFinishDeserializing(PlayerPositionReconciliationChecker);
            break;
        }
    }*/

    private void PlayerPositionReconciliationChecker(float[] playerInput){

        float timeDiff =Mathf.Abs( PongManager.Instance.GetTime() - playerInput[1]);
        float timeMargim = timeDiff * 0.1f;
        float timeStamp = playerInput[1] + timeDiff;
        int posIndex = -1;

        for (int i = 0; i < playerInputs.Count; i++){
            if(playerInputs[i].z >= timeStamp - timeMargim && playerInputs[i].z <= timeStamp + timeMargim){
                Vector3 pInput = playerInputs[i];
                pInput.y = playerInput[0];
                playerInputs[i] = pInput;
                posIndex = i;
                Reconciliation(posIndex, playerInput);
                break;
            }
        }

        if(posIndex != -1){
            for (int i = 0; i < posIndex; i++){
                playerInputs.RemoveAt(i);
            }
        }
    }

    private void Reconciliation(int posIndex, float[] playerInput){
        if(Mathf.Abs(playerInputs[posIndex].y - playerInput[0]) > maxDistanceBetweenPositions){
            float newPos = playerInput[0];
            for (int i = 0; i < playerInputs.Count; i++){
                newPos += playerInputs[i].y;
            }
            transform.position = new Vector3(transform.position.x, newPos, transform.position.z);
            SendInfo();
        }
    }
}
