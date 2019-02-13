using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : MonoBehaviour
{
    public static Dragon instance;
    public int MaxHealth = 10000;
    public int Health = 10000; 
    DragonState state = DragonState.Idle; 
    public Material Idle;
    public Material Walk1;
    public Material Walk2;
    public Material Walk3;

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

    public GameObject SpritePlane;
    public GameObject FlameSpritePlane; 
    Renderer SpriteR;
    Renderer FameSpriteR; 
    Rigidbody RB;
    Vector3 MoveDirection = Vector3.zero;
    public List<Transform> HitLocations; 

    private void Awake()
    {
        instance = this; 

    }
    void Start()
    {
        //currentSprite = Idle;
        state = DragonState.Walking;
        SpriteR = SpritePlane.GetComponent<Renderer>();
        FameSpriteR = FlameSpritePlane.GetComponent<Renderer>();
        RB = gameObject.GetComponent<Rigidbody>();
        FlameSource = gameObject.AddComponent<AudioSource>(); 

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        PlayerInput();
        RB.velocity = MoveDirection * MoveSpeed;

        UpdateSprite();
    }

    public void TakeDamage(int damage, Vector3 location)
    {
        if (damage < 1)
        { return; }

        Health -= damage;
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
        MoveDirection = Vector3.zero;
        // Ignore input while fire breathing
        if (state == DragonState.FireBreathing)
        { return; }

        
        if (Input.GetKey(KeyCode.Space))
        { DoBreathAttack(); return;  }


        state = DragonState.Idle;

        if (Input.GetKey(KeyCode.I))
        { OnMoveUp(); }
        if (Input.GetKey(KeyCode.K))
        { OnMoveDown(); }
        if (Input.GetKey(KeyCode.J))
        { OnMoveLeft(); }
        if (Input.GetKey(KeyCode.L))
        { OnMoveRight(); ; }
     

    }

    public void OnMoveUp()
    {
        MoveDirection = Vector3.forward;
        state = DragonState.Walking;
    }
    public void OnMoveDown()
    {
        MoveDirection = Vector3.back;
        state = DragonState.Walking;
    }
    public void OnMoveLeft()
    {
        MoveDirection = Vector3.left;
        state = DragonState.Walking;
    }
    public void OnMoveRight()
    {
        MoveDirection = Vector3.right;
        state = DragonState.Walking;
    }
    public void OnStopMove()
    {
       
    }

    void DoBreathAttack()
    {
        state = DragonState.FireBreathing;
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