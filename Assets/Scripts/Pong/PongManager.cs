using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using UnityEngine;

public class PongManager : MBSingleton<PongManager>
{
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

    // Start is called before the first frame update
    void Start()
    {
        // playerPointsText.SetText(playerPoints.ToString());
        // playerUDPPointsText.SetText(playerUDPPoints.ToString());
    }

    public void InitGame()
    {
        isServer = NetworkManager.Instance.isServer;
        networkMenu.SetActive(false);
        gameHud.SetActive(true);
        playerOne.SetActive(true);
        playerUDP.SetActive(true);
        SetPlayers();
        AddListener();
    }

    private void Awake() {
        networkMenu.SetActive(true);
        gameHud.SetActive(false);
        playerOne.SetActive(false);
        playerUDP.SetActive(false);
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
            PacketManager.Instance.Awake();
            playerUDPPointsText.AddListener();
            playerPointsText.AddListener();
        }
    }

    void OnDisable()
    {
        if(!isServer){
            //PacketManager.Instance.RemoveListener((uint)playerPointsText.GetInstanceID());
            //PacketManager.Instance.RemoveListener((uint)playerUDPPointsText.GetInstanceID());
        }
    }
}
