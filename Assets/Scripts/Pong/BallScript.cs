using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    private uint OwnerBallID = 2;

    private Rigidbody rb;
    public float speed = 5;
    public float roof = 4.75f;
    public float floor = -4.75f;
    private float speedX;
    private float speedY;
    // Start is called before the first frame update
    void Start()
    {
        speedX = speed;
        speedY = speed;
    }

    // Update is called once per frame
    void Update()
    {
       CheckBoundaries();
       transform.Translate(speedX * Time.deltaTime, speedY * Time.deltaTime, 0);
       SendInfo();
    }

    private void SendInfo()
    {
        MessageManager.Instance.SendPosition(transform.position, OwnerBallID);
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

    private void ResetBall()
    {
        if(speedX > 0){
            PongManager.Instance.playerPoint();
        }else{
            PongManager.Instance.PlayerUDPPoint();
        }
        transform.position = Vector3.zero;
        transform.Translate(speedX * Time.deltaTime, speedY * Time.deltaTime, 0);
    }

    private void TakePlayerImpulse()
    {
        speedX *= -1; 
    }
}
