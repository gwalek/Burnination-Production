using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class HUD : MonoBehaviour
{
    public GameObject GamePannel;
    public GameObject PreGamePannel;
    public GameObject PostGamePannel;

    public List<GameObject> PannelList; 
   
    public Text HealthValue;
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

    void UpdateGamePannels()
    {
        HealthValue.text = (( (float)Dragon.instance.Health/ (float)Dragon.instance.MaxHealth)*100).ToString()+"%";
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
