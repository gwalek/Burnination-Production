using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public GameObject SpritePlane; 
    public float ArrowSpeed = 10;
    Vector3 MoveDirection = Vector3.zero;
    Vector3 HitLocation = Vector3.zero;
    int damage = 0;
    public Controller Owner; 


    // Update is called once per frame
    void Update()
    {
        transform.position += (MoveDirection* ArrowSpeed * Time.deltaTime);
        if (IsClosetoHitPoint())
        {
            Owner.DamageDelt += damage;
            Dragon.instance.TakeDamage(damage, HitLocation); 
            Destroy(gameObject); 
            

        }
    }

    bool IsClosetoHitPoint ()
    {
        return ((HitLocation - transform.position).magnitude < (ArrowSpeed * Time.deltaTime * 3)); 
    }

    public void SetHitLocation(Vector3 hp)
    {
        HitLocation = hp;
        MoveDirection = (HitLocation - transform.position);
        float distance = MoveDirection.magnitude;
        MoveDirection = MoveDirection.normalized; 
        if (MoveDirection.x < 0)
        {
            Vector3 Xscale = SpritePlane.transform.localScale;
            Xscale.x *= -1;
            SpritePlane.transform.localScale = Xscale; 
        }

        damage = DetermineDamage(distance); 

    }

    int DetermineDamage(float distance)
    {
        // Base Range via Distance
        damage = 2; // Less than 8 close range 
        if (distance > 8) { damage = 1; } // Mid Range
        if (distance > 16) { damage = 0; } // To Far out

        // Bonus Damage chance
        // If doing damage, Roll D20.. 19 and 20 give +1 damage
        int roll = Random.Range(1, 20); 
        if (( damage != 0) && (roll >= 19) ) 
        {
            damage++; 
        }

        return damage; 
    }
}
