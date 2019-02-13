using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{

    public virtual void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("YES1");
        Gnome g = other.transform.gameObject.GetComponentInParent<Gnome>(); 
        if (g)
        {
            Debug.Log("YES2");
            if (TriggerEntered(other))
            {
                Destroy(gameObject); 
            }
        }
    }

    public virtual bool TriggerEntered(Collider other)
    {
        return true; 
    }
}
