using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : Projectile
{
    void Start()
    {
        Destroy(gameObject, 5);
    }

    void Update()
    {
        transform.position += transform.right * 5 * Time.deltaTime;
    }

    public override void OnTriggerEnter(Collider other)
    {
        if ((other.GetComponent<Entity>() && other.GetComponent<Entity>().isOwnUnit) || other.GetComponent<Projectile>()) {
            Physics.IgnoreCollision(GetComponent<BoxCollider>(), other);
        }
        if (other.GetComponent<Entity>() && other.gameObject == target) {
            other.GetComponent<Entity>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}