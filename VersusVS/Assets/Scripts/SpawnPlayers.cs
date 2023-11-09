using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayers : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject player;

    void Start()
    {
        GameObject p = Instantiate(player, Vector3.zero, this.transform.rotation, null);
        //p.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
