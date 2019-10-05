using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
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

    // Start is called before the first frame update
    void Start()
    {
        playerPointsText.text = playerPoints.ToString();
        playerUDPPointsText.text = playerUDPPoints.ToString();
    }

    private void Awake() {
        SetPlayers();
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
        playerPoints++;
        playerPointsText.text = playerPoints.ToString();
    }

    public void PlayerUDPPoint()
    {
        playerUDPPoints++;
        playerUDPPointsText.text = playerUDPPoints.ToString();
    }
}
