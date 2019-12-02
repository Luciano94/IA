using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : ReliableOrderPacket<float[]>
{
    private uint OwnerBallID = 2;

    public float speed = 5;
    public float roof = 4.75f;
    public float floor = -4.75f;

    private float speedX;
    private float speedY;
    private bool needsReset = false;

    void Start()
    {
        speedX = speed;
        speedY = speed;
    }

    void FixedUpdate()
    {
       CheckBoundaries();
       transform.Translate(speedX * Time.fixedDeltaTime, speedY * Time.fixedDeltaTime, 0);
       SendInfo();
    }

    private void SendInfo()
    {
        float[] ballInput = new float[4];
        ballInput[0] = speedX;
        ballInput[1] = speedY;

        ballInput[2] = PongManager.Instance.GetTime();
        ballInput[3] = needsReset? 1f : 0f;

        needsReset = false;

        MessageManager.Instance.SendBallPosition(ballInput, OwnerBallID, ++lastIdSent);
    }

    private void CheckBoundaries(){
        if(transform.position.y > roof)
        {
            speedY *= -1;
        }
        if(transform.position.y < floor)
        {
            speedY *= -1;
        }
    }

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.CompareTag("Player"))
        {
            TakePlayerImpulse();
        }

        if(other.gameObject.CompareTag("Respawn"))
        {
            ResetBall();
        }
    }

    private void ResetBall(){

        if(speedX > 0){
            PongManager.Instance.playerPoint();
        }else{
            PongManager.Instance.PlayerUDPPoint();
        }
        transform.position = Vector3.zero;
        transform.Translate(speedX * Time.fixedDeltaTime, speedY * Time.fixedDeltaTime, 0);
        needsReset = true;
    }

    private void TakePlayerImpulse()
    {
        speedX *= -1; 
    }
}
