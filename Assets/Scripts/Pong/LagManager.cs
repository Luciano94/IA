using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerInput
{
    public Vector2 playerPos;
    public float time;
}

public class LagManager : MBSingleton<LagManager>
{
    private uint OwnerPlayerID = 1;
    //Inputs
    private Stack<PlayerInput> playerInputs;
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
        playerInputs = new Stack<PlayerInput>();
        gameClock = 0.0f;
        actualTime = 0;
        initTime = false;

    }

    private void FixedUpdate(){
        if(initTime){
            gameClock += Time.fixedDeltaTime;
            actualTime++;
            if(actualTime >= timePerAction){
                playerI.playerPos.x = player.position.x;
                playerI.playerPos.y = player.position.y;
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
      //  MessageManager.Instance.SendInput(playerI, OwnerPlayerID);
    }

    public void Reconcilation(){
        //MAGIA LOCA
    }
}
