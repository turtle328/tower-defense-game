using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

    TextMesh tm;

	// Use this for initialization
	void Start () {
        tm = GetComponent<TextMesh>();
	}
	
	// Update is called once per frame
	void Update () {
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
            tm.text = tm.text.Remove(tm.text.Length - 1);
        else
        {
            Destroy(transform.parent.gameObject);
            LevelManager.getInstance().die();
        }
    }
}
