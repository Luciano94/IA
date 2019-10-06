using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using UnityEngine;

public class PongManager : MBSingleton<PongManager>
{
    public Text playerPointsText;
    public Text playerUDPPointsText;

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
        playerPointsText.text = playerPoints.ToString();
        playerUDPPointsText.text = playerUDPPoints.ToString();
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
            playerPointsText.text = playerPoints.ToString();
            MessageManager.Instance.SendString(playerPoints.ToString(),100);//(uint)playerPointsText.GetInstanceID());
        }
    }

    public void PlayerUDPPoint()
    {
        if(isServer){
            playerUDPPoints++;
            playerUDPPointsText.text = playerUDPPoints.ToString();
            //MessageManager.Instance.SendString(playerUDPPoints.ToString(),150);//(uint)playerUDPPointsText.GetInstanceID());
        }
    }


    //CLIENT SIDE
    void AddListener()
    {
        if(!isServer){
            PacketManager.Instance.Awake(); //HECHO POR MARTIN MONTERROSA, EL INSPECTOR DE "PAQUETES"
            PacketManager.Instance.AddListener(100, OnReceivePacket);
           // PacketManager.Instance.AddListener(150, OnReceivePacket);
        }
    }

    void OnDisable()
    {
        if(!isServer){
            PacketManager.Instance.RemoveListener((uint)playerPointsText.GetInstanceID());
            PacketManager.Instance.RemoveListener((uint)playerUDPPointsText.GetInstanceID());
        }
    }

    void OnReceivePacket(uint packetId, ushort type, Stream stream)
    {
        switch (type)
        {
            case (ushort)UserPacketType.Message:
                MessagePacket messagePacket = new MessagePacket();
                messagePacket.Deserialize(stream);
                if(packetId == 100){ //playerPointsText.GetInstanceID()){
                    playerPointsText.text = messagePacket.payload;
                }else if(packetId == 150){// playerUDPPointsText.GetInstanceID()){
                    playerUDPPointsText.text = messagePacket.payload;
                }
            break;
        }
    }
}
