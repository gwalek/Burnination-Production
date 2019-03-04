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
    List<Controller> ChamptionList = new List<Controller>();

    // Floors
    public int BestArrowfloor = 1;  // floor of 1
    public int BestDamagefloor = 1;  // Floor of 1
    public float BestD2Afloor = 1f;  // Floor of 1
    public float WorstD2Afloor = 5f; // Start High
    public int MostStompsfloor = 1; // floor of 1
    public int MostBBQfloor = 1; // floor of 1
    public int MostDeathsfloor = 1;  // floor of 1

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
        HUD.instance.PreGamePannel.SetActive(true);
        HUD.instance.PostGamePannel.SetActive(false);
        HUD.instance.GamePannel.SetActive(false);
        HUD.instance.SubBar.SetActive(false);


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
        HUD.instance.PreGamePannel.SetActive(false);
        HUD.instance.PostGamePannel.SetActive(true);
        HUD.instance.GamePannel.SetActive(false);
        HUD.instance.SubBar.SetActive(false);
        // Music Was destroyed already
        CurrentMusic = Instantiate(Music_PostGame);
        gameStatus = GameStatus.PostGame;

        foreach (KeyValuePair<int, Controller> entry in PlayerTable)
        {
            entry.Value.PostGame();
        }

        FindTitleHolders();
        SendDisplayTitles(); 

    }

    public void FindTitleHolders()
    {
        ChamptionList.Add(GotLastHit); //TitlesFlagEnum.TheChampion
        ChamptionList.Add(null); // TitlesFlagEnum.BigBadBruiser
        ChamptionList.Add(null); // TitlesFlagEnum.TheSunBlocker
        ChamptionList.Add(null); // TitlesFlagEnum.RobinOfGnomley
        ChamptionList.Add(null); // TitlesFlagEnum.HadABadBow
        ChamptionList.Add(null); // TitlesFlagEnum.CaptainFlapjack
        ChamptionList.Add(null); // TitlesFlagEnum.FlambeauPitMaster
        ChamptionList.Add(null); // TitlesFlagEnum.PhoenixRise
        ChamptionList.Add(null); // TitlesFlagEnum.TotalyDisposable


        int BestArrow = BestArrowfloor;  // floor of 1
        int BestDamage = BestDamagefloor;  // Floor of 1
        float BestD2A = BestD2Afloor;  // Floor of 1
        float WorstD2A = WorstD2Afloor; // Start High
        int MostStomps = MostStompsfloor; // floor of 1
        int MostBBQ = MostBBQfloor; // floor of 1
        int MostDeaths = MostDeathsfloor;  // floor of 1

        foreach (KeyValuePair<int, Controller> entry in PlayerTable)
        {
            Controller c = entry.Value;
            if (!c.IsMonster)
            {
                if ((c.DamageDelt > BestDamage) && (c.DamageDelt > BestDamagefloor))
                {
                    BestDamage = c.DamageDelt;
                    ChamptionList[1] = c;
                }
                if ((c.ArrowShot > BestArrow) && (c.ArrowShot > BestArrowfloor) )
                {
                    BestArrow = c.ArrowShot;
                    ChamptionList[2] = c;
                }
                if ( (c.DamagePerArrow> BestD2A) && (c.DamagePerArrow > BestD2A))
                {
                    BestD2A = c.DamagePerArrow; 
                    ChamptionList[3] = c;
                }
                if ((c.DamagePerArrow < WorstD2A) && (c.DamagePerArrow > 0))
                {
                    WorstD2A = c.DamagePerArrow; 
                    ChamptionList[4] = c;
                }
                if ((c.DeathsByStomp > MostStomps) && (c.DeathsByStomp > MostStompsfloor))
                {
                    MostStomps = c.DeathsByStomp;
                    ChamptionList[5] = c;
                }
                if ((c.DeathsByFire > MostBBQ) && (c.DeathsByFire > MostBBQfloor))
                {
                    MostBBQ = c.DeathsByFire;
                    ChamptionList[6] = c;
                }
                if ((c.Deaths > MostDeaths) && (c.Deaths > MostDeathsfloor))
                {
                    MostDeaths = c.Deaths;
                    ChamptionList[7] = c;
                }
            }

        }

        // If Best D2A is same as Worse, then Null out Worst. 
        if (ChamptionList[3] == ChamptionList[4])
        {
            ChamptionList[4] = null; 
        }

        // Most Disposable Check 
        if ((ChamptionList[7] == ChamptionList[5]) && (ChamptionList[7] == ChamptionList[6]))
        {
            ChamptionList[8] = ChamptionList[7];
        }
    }

    public void SendDisplayTitles()
    {

        SetTitle(TitlesFlagEnum.TheChampion, ChamptionList[0]);
        SetTitle(TitlesFlagEnum.BigBadBruiser, ChamptionList[1]);
        SetTitle(TitlesFlagEnum.TheSunBlocker, ChamptionList[2]);
        SetTitle(TitlesFlagEnum.RobinOfGnomley, ChamptionList[3]);
        SetTitle(TitlesFlagEnum.HadABadBow, ChamptionList[4]);
        SetTitle(TitlesFlagEnum.CaptainFlapjack, ChamptionList[5]);
        SetTitle(TitlesFlagEnum.FlambeauPitMaster, ChamptionList[6]);
        SetTitle(TitlesFlagEnum.PhoenixRise, ChamptionList[7]);
        SetTitle(TitlesFlagEnum.TotalyDisposable, ChamptionList[8]);

        foreach (KeyValuePair<int, Controller> entry in PlayerTable)
        {
            if (entry.Value.IsMonster)
            {
                // TODO: 
                // Will have to make a title for the master later on... 

            }
            else
            {
                if (entry.Value.TitleFlags == 0)
                {

                    entry.Value.AddTitle(TitlesFlagEnum.CameToPlay);
                }
                entry.Value.SendTitles();
            }
        }
    }

    public void SetTitle(TitlesFlagEnum title, Controller controller)
    {
        HUD.instance.SetTitle(title, controller);
        if (controller)
        {
            controller.AddTitle(title);
        }

    }


    public void MainMenu()
    {
        Debug.Log("MainMenu called");
        HUD.instance.PreGamePannel.SetActive(true);
        HUD.instance.PostGamePannel.SetActive(false);
        HUD.instance.GamePannel.SetActive(false);
        HUD.instance.SubBar.SetActive(false);
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
        HUD.instance.PreGamePannel.SetActive(false);
        HUD.instance.PostGamePannel.SetActive(false);
        HUD.instance.GamePannel.SetActive(true);
        HUD.instance.SubBar.SetActive(true);
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
        c.IsMonster = true;
        Dragon.instance.HasController = true; 

    }

    void Update()
    {

    }
}
