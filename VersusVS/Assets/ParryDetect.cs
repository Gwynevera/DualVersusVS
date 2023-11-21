using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryDetect : MonoBehaviour
{
    Player p;
    public LayerMask parryCollisionLayer;

    private void Start()
    {
        p = transform.parent.GetComponent<Player>();
    }
}
