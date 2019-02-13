using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollowDragon : MonoBehaviour
{
    public GameObject Dragon;
    public float CameraOffset = 5f; 
    
    // Update is called once per frame
    void Update()
    {
        Vector3 camPostion = transform.position;
        camPostion.x = Dragon.transform.position.x - CameraOffset;
        transform.position = camPostion; 
    }
}
