using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HappyNotHappyFace : HappyStarFace
{
    private Rigidbody _RB;

    public Rigidbody RB
    {
         get { return _RB; }
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        _RB = gameObject.AddComponent<Rigidbody>();
        _RB.constraints = RigidbodyConstraints.FreezeRotation;

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
