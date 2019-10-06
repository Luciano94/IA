using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using UnityEngine;

public class PongManager : MBSingleton<PongManager>
{
    public const uint gameStartOwnerId = 6;

    public TextUDP playerPointsText;
    public TextUDP playerUDPPointsText;

    private int playerPoints = 0;
    private int playerUDPPoints = 0;

    public bool isServer = true;
    public GameObject playerOne;
    public GameObject playerUDP;
    public GameObject ball;

    public GameObject networkMenu;
    public GameObject gameHud;

    public GameObject StartGameHud;
    public GameObject serverStartButton;
    public GameObject clientWaitingText;

    SynchronousGameStart gameStart;

    public void InitGame()
    {
        isServer = NetworkManager.Instance.isServer;
        networkMenu.SetActive(false);
        SetGame();
    }

    public new void Awake()
    {
        base.Awake();
        networkMenu.SetActive(true);
        StartGameHud.SetActive(false);
        gameHud.SetActive(false);
        playerOne.SetActive(false);
        playerUDP.SetActive(false);
    }

    public void StartGame()
    {
        StartGameHud.SetActive(false);
        playerOne.SetActive(true);
        playerUDP.SetActive(true);
        ball.SetActive(true);
        gameHud.SetActive(true);
        SetPlayers();
        AddListener();
    }

    public void SetGame()
    {
        gameStart = GetComponent<SynchronousGameStart>();
        StartGameHud.SetActive(true);
        if(isServer){
            serverStartButton.SetActive(true);
            clientWaitingText.SetActive(false);
            serverStartButton.GetComponent<Button>().onClick.AddListener(onClickStartButton);

        }else{
            serverStartButton.SetActive(false);
            clientWaitingText.SetActive(true);
            gameStart.AddListener();
        }
    }

    private void onClickStartButton()
    {
        gameStart.SendState(GameState.GameStart);
        StartGame();
    }

    public void SetPlayers()
    {
        if(isServer){
            playerOne.AddComponent<PlayerScript>();
            playerUDP.AddComponent<PlayerUDP>();
            ball.AddComponent<BallScript>();
        }else{
            playerOne.AddComponent<PlayerUDP>();
            playerUDP.AddComponent<PlayerScript>();
            ball.AddComponent<BallUDP>();
        }
    }

    public void playerPoint()
    {
        if(isServer){
            playerPoints++;
            playerPointsText.SetText(playerPoints.ToString());
        }
    }

    public void PlayerUDPPoint()
    {
        if(isServer){
            playerUDPPoints++;
            playerUDPPointsText.SetText(playerUDPPoints.ToString());
        }
    }


    //CLIENT SIDE
    void AddListener()
    {
        if(!isServer){
            playerUDPPointsText.AddListener();
            playerPointsText.AddListener();
        }
    }
}
