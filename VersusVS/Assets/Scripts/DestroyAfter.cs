using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    public float timeToDie = 1;
    float dieTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        dieTimer += Time.fixedDeltaTime;
        if (dieTimer > timeToDie)
        {
            Destroy(this.gameObject);
        }
    }
}
