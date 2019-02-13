using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Gnome : MonoBehaviour
{
    public Controller Master; 
    public bool IsDead = false;
    public Transform SpawnPoint;
    public GameObject SpawnPrefab; 
    GnomeState state = GnomeState.Idle; 

    public Material Idle;
    public Material Walk1;
    public Material Walk2;
    public Material DeadStomp;
    public Material DeadFire1;
    public Material DeadFire2;
    public Material DeadFire3;
    Material currentSprite;

    public List<AudioClip> StompSounds;
    public AudioClip Wilhelm;
    public AudioClip ArrowShot;


    float deathCounter = 0f;
    public float MaxDeathTime = 15f;
    public float MaxDeathTimeStomp = 15f;
    public float MaxDeathTimeFire = 25f; 

    float animationTimeCounter = 0f;
    public float AnimationBoost = 6f; 
    public int maxFrames = 6;

    public float MoveSpeed = 10f; 
    Vector3 MoveDirection = Vector3.zero;
    Vector3 FaceForward = new Vector3(-1, 1, 1);
    Vector3 FaceBack = Vector3.one; 

    public GameObject SpritePlane;
    public float SpritePlaneSize = .1f;
    Renderer SpriteR;
    Rigidbody RB;

    AudioSource DeathSource;
 

    // Start is called before the first frame update
    void Start()
    {
        currentSprite = Idle;
        state = GnomeState.Idle;
        SpriteR = SpritePlane.GetComponent<Renderer>();
        RB = gameObject.GetComponent<Rigidbody>();
        DeathSource = gameObject.AddComponent<AudioSource>();
        
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!IsDead)
        {
            UpdateAlive();
        }
        else
        {
            UpdateDeath(); 
        }

        UpdateSprite(); 
    }

    void UpdateAlive()
    {

        //DebugAnimationStates();
        //GetInputDebug();

        RB.velocity = MoveDirection * MoveSpeed;

    }

    void UpdateDeath()
    {
        deathCounter += Time.fixedDeltaTime; 
        if (deathCounter > MaxDeathTime)
        {
            Destroy(gameObject); 
        }
    }


    void GetInputDebug()
    {
        if (Input.GetKey(KeyCode.W))
        { OnMoveUp();  }
        if (Input.GetKey(KeyCode.S))
        { OnMoveDown();  }
        if (Input.GetKey(KeyCode.A))
        { OnMoveLeft();  }
        if (Input.GetKey(KeyCode.D))
        { OnMoveRight();  }
        if (Input.GetKey(KeyCode.X))
        { OnStopMove(); }
        if (Input.GetKeyDown(KeyCode.Q))
        { OnShoot(); }
    }

    public void OnMoveUp()
    {
        MoveDirection = Vector3.forward;
        state = GnomeState.Walking;
    }
    public void OnMoveDown()
    {
        MoveDirection = Vector3.back;
        state = GnomeState.Walking;
    }
    public void OnMoveLeft()
    {
        MoveDirection = Vector3.left;
        SpritePlane.transform.localScale = FaceBack * SpritePlaneSize;
        state = GnomeState.Walking;
    }
    public void OnMoveRight()
    {
        MoveDirection = Vector3.right;
        SpritePlane.transform.localScale = FaceForward * SpritePlaneSize;
        state = GnomeState.Walking;
    }
    public void OnStopMove()
    {
        MoveDirection = Vector3.zero;
        state = GnomeState.Idle;
    }
    public void OnShoot()
    {
        GameObject newArrow = Instantiate(SpawnPrefab, SpawnPoint.position, SpawnPoint.rotation);
        Arrow a = newArrow.GetComponent<Arrow>();
        a.SetHitLocation(Dragon.instance.GetHitLocation());
        DeathSource.PlayOneShot(ArrowShot, .33f);
        a.Owner = Master;
        Master.ArrowShot++; 
 
    }
    public void OnShield()
    {

    }
    public void OnBucket()
    {

    }
    public void OnBuild()
    {

    }

    public void OnDeathFire()
    {
        if (state == (GnomeState.DeadFire | GnomeState.DeadStomp))
        { return;  } // We're already dead!

        state = GnomeState.DeadFire;
        IsDead = true;
        MoveDirection = Vector3.zero;
        DeathSource.PlayOneShot(Wilhelm, .7f);
        Master.Deaths++;
        Master.DeathsByFire++;
        Master.DeadGnome();
    }
    public void OnDeathStomp()
    {
        if (state == (GnomeState.DeadFire | GnomeState.DeadStomp))
        { return; } // We're already dead!

        state = GnomeState.DeadStomp;
        PlayStompSound();
        IsDead = true;
        MoveDirection = Vector3.zero;
        Master.Deaths++;
        Master.DeathsByStomp++;
        Master.DeadGnome();
    }


    void PlayStompSound()
    {
        int index = Random.Range(1, StompSounds.Count) -1;
        DeathSource.PlayOneShot(StompSounds[index]);
    }

    void DebugAnimationStates()
    {
        if (Input.GetKey(KeyCode.Q))
        { state = GnomeState.Idle; }
        if (Input.GetKey(KeyCode.W))
        { state = GnomeState.Walking; }
        if (Input.GetKey(KeyCode.E))
        { state = GnomeState.DeadStomp; }
        if (Input.GetKey(KeyCode.R))
        { state = GnomeState.DeadFire; }
    }

    void UpdateSprite()
    {
        animationTimeCounter += (Time.fixedDeltaTime * AnimationBoost); 
        if (animationTimeCounter > maxFrames)
        { animationTimeCounter -= maxFrames;  }
        int frame = (int)animationTimeCounter;

        switch (state)
        {
            case GnomeState.Idle:
                currentSprite = Idle;
                break;
            case GnomeState.Walking:
                frame = frame % 2; 
                if (frame == 0) { currentSprite = Walk1; }
                if (frame == 1) { currentSprite = Walk2; }
                break;
            case GnomeState.DeadStomp:
                currentSprite = DeadStomp;
                break;
            case GnomeState.DeadFire:
                frame = frame % 3;
                if (frame == 0) { currentSprite = DeadFire1; }
                if (frame == 1) { currentSprite = DeadFire2; }
                if (frame == 2) { currentSprite = DeadFire3; }
                break;
        }

        Material[] mats = SpriteR.materials;
        mats[0] = currentSprite;
        SpriteR.materials = mats;
    }
}
