using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour {
    public GameObject bulletPrefab;
    public int towerCost = 100;
    public float rotationSpeed = 35;
    public float cooldown = 2;
    private float curCooldown = 0;

    // Update is called once per frame
    void Update ()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed, Space.World);
        curCooldown -= Time.deltaTime;
	}

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Monster>() && curCooldown <= 0)
        {
            curCooldown = cooldown;
            GameObject g = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            g.GetComponent<Bullet>().target = other.transform;
        }
    }
}
