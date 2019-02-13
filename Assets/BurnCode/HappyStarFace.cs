using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HappyStarFace : Pickup
{
    public AudioClip Sound;
    protected AudioSource Deathsource; 
    public override void Start()
    {
        base.Start();
        Deathsource = gameObject.AddComponent<AudioSource>(); 
    }

    public override bool TriggerEntered(Collider other)
    {
        Deathsource.PlayOneShot(Sound); 
        // Dont Destroy Flower 
        return true; 
    }
}
