using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;

public class HUD : MonoBehaviour
{
    public static HUD instance; 
    public GameObject GamePannel;
    public GameObject PreGamePannel;
    public GameObject PostGamePannel;
    public GameObject SubBar; 

    public List<GameObject> PannelList;
    public List<GameObject> ChampionPanelList;
    public List<RawImage> ChampionImageList;
    public List<Text> ChampionNickName;

    public GameObject HealthValue;
    public Vector3 HudHeathVector = Vector3.one; 
    public Text HouseBurnCount;
    public Text HouseEatCount;
    public Text GnomeFireDeaths;
    public Text GnomeStompDeaths;
    public Text ArrowsShot;
    public Text DamageDone;
    public Text SessionCode; 

    public float counter = 0;
    public float MaxTime = 10f;
    public int PannelIndex = 0;

    private void Awake()
    {
        instance = this; 
    }

    void Start()
    {
        NextPannel(); 
    }

    // Update is called once per frame
    void Update()
    {
        counter += Time.deltaTime; 
        if (counter > MaxTime)
        {
            counter = 0;
            UpdateGamePannels();
            NextPannel(); 
        }
        SessionCode.text = BurnLogic.instance.sessionCode; 

    }

    public void SetTitle (TitlesFlagEnum t, Controller c)
    {
        int index = (int)t;
        if (!c)
        {
            ChampionPanelList[index].SetActive(false);
            return; 
        }

        ChampionPanelList[index].SetActive(true);
        string nickname = AirConsole.instance.GetNickname(c.deviceID); 
        ChampionNickName[index].text = nickname; 

        if (!c.profileTexture)
        {
            ChampionImageList[index].enabled = false;
            return; 
        }
        ChampionImageList[index].enabled = true;
        ChampionImageList[index].texture = c.profileTexture; 
    }

    void UpdateGamePannels()
    {
        HudHeathVector.x = ((float)Dragon.instance.Health / (float)Dragon.instance.MaxHealth);
        HealthValue.transform.localScale = HudHeathVector;
        HouseBurnCount.text = BurnLogic.instance.HousesBurned.ToString();
        HouseEatCount.text = BurnLogic.instance.HousesEaten.ToString();

        int FireDeathCount = 0;
        int StompDeathCount = 0;
        int ArrowsCount = 0;
        int DamageCount = 0;

       foreach (KeyValuePair<int, Controller> entry in BurnLogic.instance.PlayerTable)
        { 
            FireDeathCount += entry.Value.DeathsByFire;
            StompDeathCount += entry.Value.DeathsByStomp;
            ArrowsCount += entry.Value.ArrowShot;
            DamageCount += entry.Value.DamageDelt;
        }

        GnomeFireDeaths.text = FireDeathCount.ToString();
        GnomeStompDeaths.text = StompDeathCount.ToString();
        ArrowsShot.text = ArrowsCount.ToString();
        DamageDone.text = DamageCount.ToString();
    
    }

    void NextPannel()
    {
        foreach ( GameObject g in PannelList)
        {
            g.SetActive(false); 
        }
        PannelIndex++;
        if (PannelIndex >= PannelList.Count )
        {
            PannelIndex = 0; 
        }
        PannelList[PannelIndex].SetActive(true); 
    }

}
