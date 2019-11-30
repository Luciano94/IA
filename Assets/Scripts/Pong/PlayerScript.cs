using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
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

    // Start is called before the first frame update
    void Start()
    {
        speed = PongManager.Instance.playerSpeed;
        boundariesVector = new Vector2(transform.position.x, 0);
        playerInputValue = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        CheckBounduaries();
    }

    private void SendInfo()
    {
        Debug.Log(playerInputValue);
        float[] playerInput = new float[2];
        playerInput[0] = transform.position.y;
        playerInput[1] = PongManager.Instance.GetTime();
        MessageManager.Instance.SendPlayerInput(playerInput, OwnerPlayerID);
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
        verticalAxis = Input.GetAxis("Vertical");
        if(verticalAxis != 0)
        {
            transform.Translate(0,verticalAxis * speed * Time.deltaTime,0);
        }

        EvaluatePlayerInfo();
    }

    private void EvaluatePlayerInfo(){
        actualTicks++;
        if(actualTicks >= packetsTicks){
            actualTicks = 0;
            SendInfo();
        } 
    } 
}
