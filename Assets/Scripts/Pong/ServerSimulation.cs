using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSimulation : MonoBehaviour
{
    PlayerUDP[] players;
    BallScript ballScripted;
    float playerSpeed;

    Time serverClock;
    Time[] playersClocks;
}
