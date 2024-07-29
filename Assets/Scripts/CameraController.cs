using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float FollowSpeed = 20f;
    public Transform target;
    

    // Camera suit le joueur
    void FixedUpdate()
    {
        Vector3 newPos = new Vector3(target.position.x, target.position.y, -10f);
        transform.position = Vector3.Slerp(transform.position, newPos, FollowSpeed*Time.deltaTime);
    }
}
