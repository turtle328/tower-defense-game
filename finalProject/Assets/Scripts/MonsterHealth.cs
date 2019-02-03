using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterHealth : MonoBehaviour
{
    TextMesh tm;
    Animator anim;
    NavMeshAgent nma;

    // Use this for initialization
    void Start()
    {
        anim = GetComponentInParent<Animator>();
        tm = GetComponent<TextMesh>();
        nma = GetComponentInParent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.forward = Camera.main.transform.forward;
    }

    public int current()
    {
        return tm.text.Length;
    }

    public void setHealth(int healthAmount)
    {
        tm = GetComponent<TextMesh>();
        string health = new string('-', healthAmount);
        tm.text = health;
    }

    public void decrease()
    {
        if (current() > 1)
        {
            anim.Play("ghost_damage");
            tm.text = tm.text.Remove(tm.text.Length - 1);
        }
        else
        {
            if (!string.IsNullOrEmpty(tm.text)) tm.text = tm.text.Remove(tm.text.Length - 1);
            nma.speed = 0;
            anim.Play("ghost_die");
            Destroy(transform.parent.gameObject, anim.GetCurrentAnimatorClipInfo(0).Length);
        }
    }
}