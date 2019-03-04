using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : Pawn
{
    public static Dragon instance;
    public bool HasController = false; 
    public int MaxHealth = 10;
    public int Health = 10000; 
    public DragonState state = DragonState.Idle; 
    public Material Idle;
    public Material Walk1;
    public Material Walk2;
    public Material Walk3;
    public Material Dead; 

    public Material Flame1;
    public Material Flame2;
    public Material Flame3;
    public Material Flame4;
    public Material Flame5;
    public Material Flame6;
    public Material Flame7;

    public GameObject FlameLight1;
    public GameObject FlameLight2;
    public GameObject FlameLight3;

    public GameObject FlameTrigger1;
    public GameObject FlameTrigger2;
    public GameObject FlameTrigger3;
    public AudioClip FlameAttack;
    public AudioClip DeadSound; 
    AudioSource FlameSource;

    public GameObject Star1;
    public GameObject Star2;
    public GameObject Star3;

    Material currentSprite;
    Material currentFlameSprite; 

    float animationTimeCounter = 0;
    float BreathTimeCounter = 0;
    public float AnimationBoost = 6;
    public int maxFrames = 35;
    List<bool> BreathStepList = new List<bool>();
    int BreathStep = 0;
    public float MoveSpeed = 4f;
    public float DyingAngle = 90f;
    public float DyingTime = 3;
    public float NoAttackUnderPercent = .01f;

    public GameObject SpritePlane;
    public GameObject FlameSpritePlane; 
    Renderer SpriteR;
    Renderer FameSpriteR; 
    Rigidbody RB;
    Vector3 MoveDirection = Vector3.zero;
    public List<Transform> HitLocations;
    public Vector3 StartingPosition;
    public Quaternion StartingRotation; 

    private void Awake()
    {
        instance = this;
        IsMonster = true; 
    }
    void Start()
    {
        //currentSprite = Idle;
        
        SpriteR = SpritePlane.GetComponent<Renderer>();
        FameSpriteR = FlameSpritePlane.GetComponent<Renderer>();
        RB = gameObject.GetComponent<Rigidbody>();
        FlameSource = gameObject.AddComponent<AudioSource>();
        StartingPosition = gameObject.transform.position;
        StartingRotation = gameObject.transform.rotation; 
        Restart(); 

    }

    public void Restart()
    {
        state = DragonState.Idle;
        Health = MaxHealth;
        gameObject.transform.position = StartingPosition;
        gameObject.transform.rotation = StartingRotation;
        HasController = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (state == DragonState.Dead)
        {
            return; 
        }


        if (state == DragonState.Dying) 
        {
            RB.velocity = Vector3.zero;
        } 
        else
        {
            if (!HasController)
            {
                PlayerInput();
            }
            
            RB.velocity = MoveDirection * MoveSpeed;
        }

        UpdateSprite();
    }

    public void TakeDamage(int damage, Vector3 location, Controller player)
    {
        // No More Damage after hitting Zero
        // Ignore arrows that don't have damage. 
        if ( (damage < 1) || (Health < 1) ) 
        { return; }

        Health -= damage;
        player.DamageDelt += damage;

        // Death Check
        if (Health <= 0)
        {
            Debug.Log("Dead");
            state = DragonState.Dying;
            BreathTimeCounter = 0;
            BurnLogic.instance.GotLastHit = player;
            Destroy(BurnLogic.instance.CurrentMusic);
            FlameSource.PlayOneShot(DeadSound); 

        }
        
        // Spawn effect based on Damage
        if (damage == 1)
        { Instantiate(Star1, location, Quaternion.identity); return;  }
        if (damage == 2)
        { Instantiate(Star2, location, Quaternion.identity); return;  }
        if (damage >= 3)
        { Instantiate(Star3, location, Quaternion.identity); return;  }

    }

    public Vector3 GetHitLocation()
    {
        int index = Random.Range(0, (HitLocations.Count-1));
        return HitLocations[index].position;
    }
  
    // Not going to worry about AI if I have these controls for now. 
    void PlayerInput()
    {
        // Ignore input while fire breathing
        if (state == DragonState.FireBreathing)
        { return; }

        
        if (Input.GetKey(KeyCode.Space))
        { DoBreathAttack(); return;  }


        state = DragonState.Idle;
        MoveDirection = Vector3.zero; 
        if (Input.GetKey(KeyCode.I))
        { OnMoveUp();   return; }
        if (Input.GetKey(KeyCode.K))
        { OnMoveDown(); return; }
        if (Input.GetKey(KeyCode.J))
        { OnMoveLeft(); return; }
        if (Input.GetKey(KeyCode.L))
        { OnMoveRight(); return; }
     

    }

    public override void OnMoveUp()
    {
        MoveDirection = Vector3.forward;
        state = DragonState.Walking;
    }
    public override void OnMoveDown()
    {
        MoveDirection = Vector3.back;
        state = DragonState.Walking;
    }
    public override void OnMoveLeft()
    {
        MoveDirection = Vector3.left;
        state = DragonState.Walking;
    }
    public override void OnMoveRight()
    {
        MoveDirection = Vector3.right;
        state = DragonState.Walking;
    }
    public override void OnStopMove()
    {
        MoveDirection = Vector3.zero;
        state = DragonState.Idle;
    }

    public override void OnShoot()
    {
        DoBreathAttack(); 
    }

    void DoBreathAttack()
    {

        if (Health <= (((float)MaxHealth) * NoAttackUnderPercent))
        {
            // Don't do a breath attack under 1%
            return;
        }
        state = DragonState.FireBreathing;
        MoveDirection = Vector3.zero;
        BreathTimeCounter = 0;
        BreathStep = 0;

        BreathStepList.Clear();
        BreathStepList.Add(true);// Pre Breath 
        BreathStepList.Add(true);// Frame 1
        BreathStepList.Add(true);// Frame 2
        BreathStepList.Add(true);// Frame 3
        BreathStepList.Add(true);// Frame 4
        BreathStepList.Add(true);// Frame 5
        BreathStepList.Add(true);// Frame 6
        BreathStepList.Add(true);// Frame 7

        FlameSource.PlayOneShot(FlameAttack, .7f);
    }


    void UpdateSprite()
    {
        animationTimeCounter += (Time.fixedDeltaTime * AnimationBoost);
        if (animationTimeCounter > maxFrames)
        { animationTimeCounter -= maxFrames; }
        int frame = (int)animationTimeCounter;

        switch (state)
        {
            case DragonState.Idle:
                currentSprite = Idle;
                break;
            case DragonState.Dying:
                AnimateDying();
                break;
            case DragonState.Walking:
                frame = frame % 5;
                if (frame == 0) { currentSprite = Walk1; }
                if (frame == 1) { currentSprite = Walk2; }
                if (frame == 2) { currentSprite = Walk3; }
                if (frame == 3) { currentSprite = Walk2; }
                if (frame == 4) { currentSprite = Walk1; }
                break;
            case DragonState.FireBreathing:
                currentSprite = Idle;
                AnimateFire(); 
                break;
        }

        Material[] mats = SpriteR.materials;
        mats[0] = currentSprite;
        SpriteR.materials = mats;
    }

    void AnimateDying()
    {
        currentSprite = Dead;
        BreathTimeCounter += Time.fixedDeltaTime;

        if (BreathTimeCounter < 1)
        {
            return;
        }

        float value = BreathTimeCounter - 1;
        float angle = value * (DyingAngle / DyingTime);
        angle = Mathf.Clamp(angle, 0f, DyingAngle); 
        Quaternion rot = Quaternion.Euler(angle,180f,0); 
        gameObject.transform.rotation = rot; 

        if (value > DyingTime)
        {
            state = DragonState.Dead;
            BurnLogic.instance.OnGameEnd();
        }
    }

    void AnimateFire()
    {
        BreathTimeCounter += Time.fixedDeltaTime;

        if (BreathTimeCounter < 1)
        {
            return;
        }

        // Set Frame 1
        if ((BreathStepList[0]) && (BreathTimeCounter >= 1f ) )
        {
            BreathStepList[0] = false;
            currentFlameSprite = Flame1;
            FlameLight1.SetActive(true);
            FlameTrigger1.SetActive(true);
            FlameSpritePlane.SetActive(true); 
        }

        // Set Frame 2 
        if ((BreathStepList[1]) && (BreathTimeCounter >= 1.2f))
        {
            BreathStepList[1] = false;
            currentFlameSprite = Flame2;
        }

        // Set Frame 3
        if ((BreathStepList[2]) && (BreathTimeCounter >= 1.4f))
        {
            BreathStepList[2] = false;
            currentFlameSprite = Flame3;
            FlameLight2.SetActive(true);
            FlameTrigger2.SetActive(true);
        }

        // Set Frame 4
        if ((BreathStepList[3]) && (BreathTimeCounter >= 1.6f))
        {
            BreathStepList[3] = false;
            currentFlameSprite = Flame4;
            FlameLight1.SetActive(false);
          
        }

        // Set Frame 5 
        if ((BreathStepList[4]) && (BreathTimeCounter >= 1.8f))
        {
            BreathStepList[4] = false;
            currentFlameSprite = Flame5;
            FlameLight3.SetActive(true);
            FlameTrigger3.SetActive(true);
            
        }

        // Set Frame 6
        if ((BreathStepList[5]) && (BreathTimeCounter >= 2.0f))
        {
            BreathStepList[5] = false;
            currentFlameSprite = Flame6;
            FlameLight2.SetActive(false);

        }

        // Set Frame 7 
        if ((BreathStepList[6]) && (BreathTimeCounter >= 2.2f))
        {
            BreathStepList[6] = false;
            currentFlameSprite = Flame7;         
        }

        // Done 
        if ((BreathStepList[7]) && (BreathTimeCounter >= 2.4f))
        {
            BreathStepList[7] = false;
            currentFlameSprite = Flame1;
            FlameLight1.SetActive(false);
            FlameLight2.SetActive(false);
            FlameLight3.SetActive(false);
            FlameTrigger1.SetActive(false);
            FlameTrigger2.SetActive(false);
            FlameTrigger3.SetActive(false);
            FlameSpritePlane.SetActive(false);
            state = DragonState.Idle;
        }



        Material[] mats = FameSpriteR.materials;
        mats[0] = currentFlameSprite;
        FameSpriteR.materials = mats;
    }
}