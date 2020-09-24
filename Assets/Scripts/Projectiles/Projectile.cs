using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage;
    public float distanceToTarget;
    public float distanceTravelled;
    public GameObject target;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void OnTriggerEnter(Collider other)
    {}
}