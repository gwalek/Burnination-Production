using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public bool UseKeyboard = false; 
    public int deviceID = -1;
     
    public Gnome pawn;

    public int Deaths=0;
    public int DeathsByFire =0;
    public int DeathsByStomp =0;
    public int DamageDelt = 0;
    public int ArrowShot = 0;
    public float DamagePerArrow = 0f;

    // For Later Expansion of the Game
    int ShieldUsed = 0;
    int BuildUsed = 0;
    int BucketUsed = 0; 


    public void DeadGnome()
    {
        pawn = null; 
        // Send Message to Device ID show dead DIV
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
    }

    public void PostGame()
    {
        // Send Message to Device ID show PostGame Div
    }

    public void PreGame()
    {
        // Send Message to Device ID show PreGame Div
    }

    public void StartGame()
    {
        // Send Message to Device ID show StartGame Div
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


    void Update()
    {
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
}
