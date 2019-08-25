using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankPlayer : TankBase 
{
    private void Start()
    {
        drawGizmos = false;
    }

    override protected void OnUpdate()
    {
        SetAxisH(Input.GetAxis("Horizontal2"));
        SetAxisV(Input.GetAxis("Vertical2"));

        if (Input.GetButtonDown("Fire1"))
        {
            Fire();
        }
    }
}