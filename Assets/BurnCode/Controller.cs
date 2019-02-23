using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking; 
using Newtonsoft.Json.Linq;
using NDream.AirConsole;

public class Controller : MonoBehaviour
{
    // DEBUG Don't use. 
    bool UseKeyboard = false;

    // Which Device are we connected to
    public int deviceID = -1;
     
    // Fresh Meat. *cough* Who's being controlled 
    public Gnome pawn;

    // Stat Tracking Variables
    public int Deaths=0;
    public int DeathsByFire =0;
    public int DeathsByStomp =0;
    public int DamageDelt_CurrentLife = 0;
    public int ArrowShot_CurrentLife = 0;
    public int DamageDelt_Best = 0;
    public int ArrowShot_Best = 0;
    public int DamageDelt = 0;
    public int ArrowShot = 0;
    public float DamagePerArrow = 0f;

    // For Later Expansion of the Game
    int ShieldUsed = 0;
    int BuildUsed = 0;
    int BucketUsed = 0;

    // Profile Image on Pawn Variables
    Texture profileTexture; 
    string url = "";
    public bool IsShowingProfileImage = false;
    public bool BlinkState = true; 
    public float ProfileTimeMax = 5.0f;
    public float ProfileBlinkToggle = .5f;
    public float ProfileToggleAfter = 2; 
    public float NextToggle = 0; 
    public float ProfileCounter = 0; 

    void Update()
    {
        if (IsShowingProfileImage)
        {
            ProfileCounter += Time.deltaTime;

            if (ProfileCounter > NextToggle)
            {
                Blinktoggle();
            }

            if (ProfileCounter> ProfileTimeMax)
            {
                EndShowingProfile();
            }
        }

        // DEBUG ONLY
        if (UseKeyboard)
        {
            if (pawn)
            {
                GetInputDebug();
            }

            if ((pawn == null) && (Input.GetKeyDown(KeyCode.Q)))
            {
                SpawnGnome();
            }

        }
    }


    public void Blinktoggle()
    {
        NextToggle += ProfileBlinkToggle;
        BlinkState = !BlinkState; 
        pawn.Profile.gameObject.SetActive(BlinkState);
    }
    public void StartGame()
    {
        // What happens when player connects (or reconnects) 
        // Send Message to Device ID show StartGame Div
    }
    public void StartShowingProfile()
    {
        IsShowingProfileImage = true;
        BlinkState = true;
        ProfileCounter = 0;
        NextToggle = ProfileToggleAfter;
        pawn.Profile.gameObject.SetActive(true);
    }

    public void EndShowingProfile()
    {
        IsShowingProfileImage = false;
        pawn.Profile.gameObject.SetActive(false);
    }

    public void DeadGnomeByFire()
    {
        DeathsByFire++;
        DeadGnome();
        //Show Dead by Fire Respawn DIV
        AirConsole.instance.Message(deviceID, "DeathFire");
    }

    public void DeadGnomeByStomp()
    {
        DeathsByStomp++;
        DeadGnome();
        //Show Dead by Stomp Respawn DIV
        AirConsole.instance.Message(deviceID, "DeathStomp");
    }

    public void DeadGnome()
    {
        Deaths++;
        EndShowingProfile();
        pawn = null;

        // Stat Tracking 
        if (DamageDelt_CurrentLife > DamageDelt_Best)
        {
            DamageDelt_Best = DamageDelt_CurrentLife;
        }
        
        if (ArrowShot_CurrentLife > ArrowShot_Best)
        {
            ArrowShot_Best = ArrowShot_CurrentLife;
        }
    }

    public void SpawnGnome()
    {
        if (pawn != null)
        { return; } // we have a gnome already! 

        Vector3 SpawnLocation = Vector3.zero;
        SpawnLocation.z = Random.Range(-4, 15);
        SpawnLocation.x = (Dragon.instance.gameObject.transform.position.x - BurnLogic.instance.Spawnoffset); 
        GameObject newG = Instantiate(BurnLogic.instance.GnomePrefab, SpawnLocation, Quaternion.identity);
        Gnome g = newG.GetComponent<Gnome>();
        pawn = g;
        g.Master = this;

        // Send Message to Device ID show Alive DIV
        AirConsole.instance.Message(deviceID, "Playing");
   
        DamageDelt_CurrentLife = 0;
        ArrowShot_CurrentLife = 0;
    }

    public void PostGame()
    {
        // Send Message to Device ID show PostGame Div
        AirConsole.instance.Message(deviceID, "PostGame");
    }

    public void Disconnect()
    {
        if (pawn)
        {
            Destroy(pawn); 
            pawn = null; 
        }
    }

    public void OnMoveUp()
    {
        if (pawn)
        {
            pawn.OnMoveUp();
        }
    }

    public void OnMoveDown()
    {
        if (pawn)
        {
            pawn.OnMoveDown();
        }
    }

    public void OnMoveLeft()
    {
        if (pawn)
        {
            pawn.OnMoveLeft();
        }
    }

    public void OnMoveRight()
    {
        if (pawn)
        {
            pawn.OnMoveRight();
        }
    }

    public void OnStopMove()
    {
        if (pawn)
        {
            pawn.OnStopMove();
        }
    }

    public void OnShoot()
    {
        if (pawn)
        {
            pawn.OnShoot();
        }
    }

    public void ShowProfileImage()
    {
        //Debug.Log(deviceID + " ShowProfile"); 
        if (!pawn)
        {
            return; 
            // No Gnome to show the image on!
        }
    
        if (profileTexture)
        {
            //Debug.Log(deviceID + " ShowProfile - has Texture and pawn");
            pawn.Profile.texture = profileTexture;
            StartShowingProfile();
        }
        else
        {
            // We don't a the profile image yet, somehow, go get it. 
            GetProfileImage(); 
        }
    }

    public void GetProfileImage()
    {       
            url = AirConsole.instance.GetProfilePicture(deviceID);
            StartCoroutine(GetProfileImagefromWeb());
    }

    IEnumerator GetProfileImagefromWeb()
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            //Debug.Log("ID " + deviceID + " Got Texture");
            profileTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
        }
    }

    void GetInputDebug()
    {
        if (Input.GetKey(KeyCode.W))
        { pawn.OnMoveUp(); }
        if (Input.GetKey(KeyCode.S))
        { pawn.OnMoveDown(); }
        if (Input.GetKey(KeyCode.A))
        { pawn.OnMoveLeft(); }
        if (Input.GetKey(KeyCode.D))
        { pawn.OnMoveRight(); }
        if (Input.GetKey(KeyCode.X))
        { pawn.OnStopMove(); }
        if (Input.GetKeyDown(KeyCode.Q))
        { pawn.OnShoot(); }
    }
}
