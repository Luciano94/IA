/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerInput
{
    public float playerPos;
    public float time;
}

public struct BallInput
{
    public Vector2 ballInputs;
    public float time;
}


public class LagManager : MBSingleton<LagManager>
{
    private uint OwnerPlayerID = 1;
    //Inputs
    private Queue<PlayerInput> playerInputs;
    private Queue<BallInput> ballInputs;
    private PlayerInput playerI;
    
    //Clock
    private float gameClock;
    bool initTime;
    
    //Interpolation
    public int timePerAction = 40;
    int actualTime;
    Transform player;

    public new void Awake(){
        base.Awake();
        playerInputs = new Queue<PlayerInput>();
        gameClock = 0.0f;
        actualTime = 0;
        initTime = false;
    }

    private void FixedUpdate(){
        if(initTime){
            gameClock += Time.fixedDeltaTime;
            actualTime++;
            if(actualTime >= timePerAction){
                playerI.playerPos = player.position.y;
                playerI.time = gameClock;
                InputSend(playerI);
            }
        }
    }

    public void StartGame(Transform _player){
        initTime = true;
        player = _player;
    }

    public void EndGame(){
        initTime = false;
    }

    public void InputSend(PlayerInput playerI){
        playerInputs.Push(playerI);
        float[] playerInput = new float[2];
        playerInput[0] = playerI.playerPos;
        playerInput[1] = playerI.time; 
        
        MessageManager.Instance.SendPlayerInput(playerInput, OwnerPlayerID);
    }

    public void Reconciliation(){
        //MAGIA LOCA
    }
}*/
