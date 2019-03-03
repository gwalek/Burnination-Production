using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;

public class BurnLogic : MonoBehaviour
{
    public GameStatus gameStatus = GameStatus.PreGame;  
    public static BurnLogic instance;
    public GameObject GnomePrefab;
    public float Spawnoffset = 16f;
    public Dictionary<int, Controller> PlayerTable;
   
    public string sessionCode = "";
    
    public GameObject CurrentMusic;
    public GameObject Music_MainMenu;
    public GameObject Music_Game;
    public GameObject Music_PostGame;

    // Session Stats
    public int HousesBurned = 0;
    public int HousesEaten = 0;
    public int HousesDestoyed = 0;
    public Controller GotLastHit;

    private void Reset()
    {
        HousesBurned = 0;
        HousesEaten = 0;
        HousesDestoyed = 0;
        GotLastHit = null;
    }

    void Awake()
    {
        instance = this;
        AirConsole.instance.onMessage += OnMessage;
        AirConsole.instance.onReady += OnReady;
        AirConsole.instance.onConnect += OnConnect;
        AirConsole.instance.onDisconnect += OnDisconnect;
        AirConsole.instance.onGameEnd += OnGameEnd;
        PlayerTable = new Dictionary<int, Controller>();
    }

    private void Start()
    {
        CurrentMusic = Instantiate(Music_MainMenu); 
    }

    void OnMessage(int deviceID, JToken data)
    {
        if (PlayerTable.ContainsKey(deviceID) && data["action"] != null)
        {
            string message = data["action"].ToString();

            if (message == "left")
            {
                GetController(deviceID).OnMoveLeft();
            }

            if (message == "right")
            {
                GetController(deviceID).OnMoveRight();
            }

            if (message == "up")
            {
                GetController(deviceID).OnMoveUp();
            }

            if (message == "down")
            {
                GetController(deviceID).OnMoveDown();
            }

            if (message == "stop")
            {
                GetController(deviceID).OnStopMove();
            }

            if (message == "shoot")
            {
                GetController(deviceID).OnShoot();
            }

            if (message == "respawn")
            {
                GetController(deviceID).Spawn();
            }

            if (message == "showprofile")
            {
                GetController(deviceID).ShowProfileImage();
            }

            if (message == "mainmenu")
            {
                MainMenu();
            }

            if (message == "startgame")
            {
                GameBegin(); 
               
            }
            if (message == "startgameasdragon")
            {
                GameBegin();
                ConntectToDragon(); 

            }
        }
    }

    void OnReady(string code)
    {
        sessionCode = code; 
    }

    void OnConnect(int deviceID)
    {
        if (!PlayerTable.ContainsKey(deviceID))
        {
            AddPlayer(deviceID); 
        }

        GetController(deviceID).StartGame(); 
    }

    void AddPlayer(int deviceID)
    {
        Controller c = gameObject.AddComponent<Controller>();
        c.deviceID = deviceID;
        c.GetProfileImage(); 
        PlayerTable.Add(deviceID, c); 
    }

    Controller GetController(int deviceID)
    {
        return PlayerTable[deviceID]; 
    }

    void OnDisconnect(int deviceID)
    {
        GetController(deviceID).StartGame();
    }

    public void OnGameEnd()
    {
        // Music Was destroyed already
        CurrentMusic = Instantiate(Music_PostGame);
        gameStatus = GameStatus.PostGame;

        foreach (KeyValuePair<int, Controller> entry in PlayerTable)
        {
            entry.Value.PostGame();
        }
    }

    public void MainMenu()
    {
        Debug.Log("MainMenu called");
        Destroy(CurrentMusic);
        CurrentMusic = Instantiate(Music_MainMenu);
        gameStatus = GameStatus.PreGame;
        foreach (KeyValuePair<int, Controller> entry in PlayerTable)
        {
            entry.Value.pawn = null; 
            entry.Value.PreGame();
        }
        Gnome[] gList = GameObject.FindObjectsOfType<Gnome>();
        Debug.Log("gList Gnomes: " + gList.Length); 
        foreach (Gnome g in gList)
        {
            Destroy(g.gameObject); 
        }
    }

    public void GameBegin()
    {
        Debug.Log("StartGame called");
        Reset(); 
        Destroy(CurrentMusic);
        CurrentMusic = Instantiate(Music_Game);
        gameStatus = GameStatus.Game;
        Dragon.instance.Restart();
        // Call restart on all the houses... 
        House[] houses = GameObject.FindObjectsOfType<House>(); 
        foreach (House h in houses)
        {
            h.Reset(); 
        }

        //Tell All Controlers to Spawn in & Reset
        foreach (KeyValuePair<int, Controller> entry in PlayerTable)
        {
            entry.Value.Reset(); 
            entry.Value.SpawnGnome(); 
        }

            // Hide the Main Menu Canvas
            // Show Game Canvas

        }

    void ConntectToDragon()
    {
        Controller c = GetController(AirConsole.instance.GetMasterControllerDeviceId());
        Destroy(c.pawn.gameObject);
        c.pawn = Dragon.instance;
        Dragon.instance.HasController = true; 

    }

    void Update()
    {

    }
}
