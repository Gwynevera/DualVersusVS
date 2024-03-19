using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraMovement : MonoBehaviour
{
    [Header("Misc Variables")]
    private GameObject[] Players;
    private float originalZ;

    [Header("Traveling")]
    [SerializeField] private float timeTraveling = 2.0f;
    public float deltaTime = 0;
  

    [Header("Camera Follow Constrains")]
    public bool followPlayers = true;
    [SerializeField] float minY = 0;
    [SerializeField] float yOffset = 0;

    [Header("Camera Zoom Constrains")]
    [SerializeField] float MaxDistance = 0; 
    [SerializeField] float MinDistance = 0; 
    

    private Camera mainCam;

    private void Start()
    {
        Players = GameObject.FindGameObjectsWithTag("Player");
        mainCam = this.GetComponent<Camera>();
        originalZ = this.transform.position.z;
    }

    private void Update()
    {
        if (followPlayers) { 
            //Camera following
            Vector3 playersMidPos = new Vector3();

            playersMidPos.x = Players[0].transform.position.x + (Players[1].transform.position.x - Players[0].transform.position.x) / 2;
            playersMidPos.y = Players[0].transform.position.y + (Players[1].transform.position.y - Players[0].transform.position.y) / 2;

            Vector3 posFinal = new Vector3(playersMidPos.x, playersMidPos.y + yOffset, originalZ);
            Vector3 posOriginal = this.transform.position;

            if (posFinal != posOriginal) {
                deltaTime += Time.deltaTime;
                float percentage = deltaTime / timeTraveling;

                if (posFinal.y < minY)
                {
                    posFinal.y = minY;
                }

                transform.position = Vector3.Lerp(posOriginal, posFinal, percentage);

                if (deltaTime >= 1 || posOriginal == posFinal)
                {
                    deltaTime = 0;
                }
            }

            //Camera Zoom
            float distanceBetweenPlayers = Vector3.Distance(Players[0].transform.position, Players[1].transform.position);

            if (distanceBetweenPlayers > MaxDistance) distanceBetweenPlayers = MaxDistance;
            if (distanceBetweenPlayers < MinDistance) distanceBetweenPlayers = MinDistance;

            mainCam.orthographicSize = distanceBetweenPlayers;
        }


    }



}
