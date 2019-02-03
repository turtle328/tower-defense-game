using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    float speed = 10;

    // tower's target
    public Transform target;

    private void FixedUpdate()
    {
        if (target)
        {
            Vector3 dir = target.position - transform.position;
            GetComponent<Rigidbody>().velocity = dir.normalized * speed;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        MonsterHealth health = other.GetComponentInChildren<MonsterHealth>();
        if (health)
        {
            health.decrease();
            Destroy(gameObject);
        }
    }
}
