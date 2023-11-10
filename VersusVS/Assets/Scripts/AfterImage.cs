using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImage : MonoBehaviour
{
    public GameObject target;
    public GameObject player;

    public Vector2 dir;
    public float speed;
    public float dist;
    float minD;

    public bool stop;
    public bool done = false;

    void Start()
    {
    }

    public void Setup(GameObject p, GameObject t, float s, float d, Sprite spr)
    {
        minD = 0.01f;
        player = p;

        target = t;
        speed = s;
        dist = d;
        stop = true;

        GetComponent<SpriteRenderer>().sprite = spr;
        GetComponent<SpriteRenderer>().color = Color.red;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target != null && !done)
        {
            if (stop)
            {
                if (Vector2.Distance(target.transform.position, transform.position) > dist)
                {
                    stop = false;
                }
            }
            else
            {
                dir = (target.transform.position - transform.position).normalized;
                GetComponent<Rigidbody2D>().velocity = dir * speed;

                if (Vector2.Distance(target.transform.position, transform.position) > minD)
                {
                    done = true;
                }
            }
        }
        if (target == null)
        {
            target = player;
        }
    }
}
