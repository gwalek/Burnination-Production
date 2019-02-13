using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;

public class BurnLogic : MonoBehaviour
{
    public static BurnLogic instance;
    public GameObject GnomePrefab;
    public float Spawnoffset = 16f;
    public Dictionary<int, Controller> PlayerTable;
    public int HousesBurned = 0;
    public int HousesEaten = 0;
    public int HousesDestoyed = 0;
    public string sessionCode = ""; 
    // Start is called before the first frame update
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
                GetController(deviceID).SpawnGnome();
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

    void OnGameEnd()
    {
    }

    void Update()
    {

    }
}
