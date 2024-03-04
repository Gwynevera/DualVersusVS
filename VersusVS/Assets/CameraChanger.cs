using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class CameraChanger : MonoBehaviour
{

    private GameObject Camera;
    [SerializeField] private Transform cameraPoint;


    void Start()
    {
        Camera = GameObject.Find("Main Camera");    
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if(collision.tag == "Player")
        {
           
            Camera.GetComponent<CameraMovement>().moveCamera(cameraPoint.position);
            
        }
    }
}
