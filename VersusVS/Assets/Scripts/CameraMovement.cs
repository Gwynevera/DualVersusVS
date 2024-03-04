using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraMovement : MonoBehaviour
{
    [Header("Traveling")]
    private Vector3 targetPosition;
    [NonSerialized]public bool lerping = false;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float timer = 0.0f;
    public float timeTraveling = 2.0f;


    [Header("Camera Zoom in/out")]
    private GameObject[] Players;
    private float distanceBetweenPlayers;
    [SerializeField] float distanceOffset = 0; 
    [SerializeField] float MaxDistance = 0; 
    [SerializeField] float MinDistance = 0; 
    [SerializeField] float yOffset = 0; 

    private Camera mainCam;

    private void Start()
    {
       Players = GameObject.FindGameObjectsWithTag("Player");
       mainCam = this.GetComponent<Camera>();
    }

    private void Update()
    {

        distanceBetweenPlayers = Vector3.Distance(Players[0].transform.position, Players[1].transform.position);

        float tmpDistance = distanceBetweenPlayers * distanceOffset;
        if (tmpDistance < MaxDistance && tmpDistance > MinDistance) mainCam.orthographicSize = tmpDistance;

        Vector2 tmpPosition;
        tmpPosition.x = Players[0].transform.position.x + (Players[1].transform.position.x - Players[0].transform.position.x) / 2;
        tmpPosition.y = Players[0].transform.position.y + (Players[1].transform.position.y - Players[0].transform.position.y) / 2;

        this.transform.position = new Vector3(tmpPosition.x, tmpPosition.y + yOffset, -10);


        if (lerping)
        {
            timer += Time.deltaTime;
            float percentage = timer / timeTraveling;

            transform.position = Vector3.Lerp(startPosition, endPosition, percentage);

            if (percentage >= 1)
            {
                lerping = false;
                timer = 0;
            }
        }        
    }


    public void moveCamera(Vector3 endPoint)
    {
        startPosition = this.transform.position;
        endPosition = endPoint;
        lerping = true;
        timer = 0.0f;

    }


}
