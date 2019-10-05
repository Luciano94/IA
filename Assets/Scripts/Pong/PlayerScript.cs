using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float verticalAxis;
    private Vector2 boundariesVector;
    public float speed = 10;
    public float floor = -3.5f;
    public float roof = 3.5f;

    // Start is called before the first frame update
    void Start()
    {
        boundariesVector = new Vector2(-8, 0);
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
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

    private void Movement()
    {
        verticalAxis = Input.GetAxis("Vertical");

        if(verticalAxis != 0)
        {
            transform.Translate(0,verticalAxis * speed * Time.deltaTime,0);
        }
    }
}
