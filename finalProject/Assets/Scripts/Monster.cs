using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour {
    public int Score { get; set; }
    public static int numMonsters = 0;
    private bool isColliding;
    private bool reachedCastle = false;
    private Animator anim;
	// Use this for initialization
	void Start ()
    {
        anim = GetComponent<Animator>();
        GameObject castle = GameObject.Find("Castle");
        if (castle)
            GetComponent<NavMeshAgent>().destination = castle.transform.position;
        numMonsters++;
	}

    private void Update()
    {
        isColliding = false;
    }

    private void OnTriggerEnter(Collider co)
    {
        if (isColliding) return;
        isColliding = true;
        if (co.name == "Castle" && !reachedCastle)
        {
            reachedCastle = true;
            anim.Play("ghost_attack");
            Destroy(gameObject, anim.GetCurrentAnimatorClipInfo(0).Length);
            co.transform.GetChild(0).GetComponent<Health>().decrease();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
        }
    }

    private void OnDestroy()
    {
        if (!reachedCastle && !LevelManager.loadingData)
            LevelManager.getInstance().addScore(Score);
        numMonsters--;
    }
}
