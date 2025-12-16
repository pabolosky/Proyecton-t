using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraFollow : MonoBehaviour
{
    public Transform target;          
    
    public float speed = 2f; 

    void Update()
    {
        Vector3 newPos = new Vector3(target.position.x, target.position.y - 1, -10f);
        transform.position = Vector3.Slerp (transform.position, newPos, 0.125f);
}
}
