using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour
{
    public GameObject SpritePlane;

    public AudioClip Nom;
    public AudioClip Ding; 

    public List<Material> HouseList;
    public List<Material> BurnList;
    public Material StompHouse; 
    Material currentSprite;

    float animationTimeCounter = 0f;
    public float AnimationBoost = 6f;
    public int maxFrames = 3;

    public bool IsOnFire = false;
    public bool isDead = false;

    Renderer SpriteR;
    
    AudioSource DeathSource;

    void Start()
    {
        SpriteR = SpritePlane.GetComponent<Renderer>();
        currentSprite = HouseList[Random.Range(0, (HouseList.Count - 1))];
        SetSprite();
        DeathSource = gameObject.AddComponent<AudioSource>(); 
    }

    void SetSprite()
    {
        Material[] mats = SpriteR.materials;
        mats[0] = currentSprite;
        SpriteR.materials = mats;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOnFire)
        { return; }

        animationTimeCounter += (Time.fixedDeltaTime * AnimationBoost);
        if (animationTimeCounter > maxFrames)
        { animationTimeCounter -= maxFrames; }
        int frame = (int)animationTimeCounter;

        frame = frame % 3;

        currentSprite = BurnList[frame];
        SetSprite(); 
    }

    public void OnDeathStomp()
    {
        if (isDead)
        { return;  }

        isDead = true;
        currentSprite = StompHouse;
        SetSprite();
        DeathSource.PlayOneShot(Nom); 
        BurnLogic.instance.HousesEaten++;
        BurnLogic.instance.HousesDestoyed++;
    }

    public void OnDeathFire()
    {
        if (isDead)
        { return; }

        isDead = true; 
        IsOnFire = true;
        DeathSource.PlayOneShot(Ding);
        BurnLogic.instance.HousesBurned++;
        BurnLogic.instance.HousesDestoyed++;
    }
}
