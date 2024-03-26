using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CameraTarget {PLAYER1, PLAYER2, CENTER}


public class CameraMovement : MonoBehaviour
{
    [Header("Camera Target")]
    public CameraTarget cameraTarget = CameraTarget.CENTER;

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
    [SerializeField] bool autoZoom = true;
    private float lastZoom;

    private Camera mainCam;
    private Transform cameraHolder;


    private void Start()
    {

        Players = GameObject.FindGameObjectsWithTag("Player");
        mainCam = this.GetComponent<Camera>();
        cameraHolder = this.transform.parent.transform;
        originalZ = cameraHolder.transform.position.z;

        StartCoroutine(CameraZoom(2, 10));

    }

    private void Update()
    {
        Vector3 CameraTargetPos = new Vector3();
        Vector3 posFinal = new Vector3();
        Vector3 posOriginal = new Vector3();

        //Camera Target
        if (cameraTarget == CameraTarget.CENTER) {
   
            CameraTargetPos.x = Players[0].transform.position.x + (Players[1].transform.position.x - Players[0].transform.position.x) / 2;
            CameraTargetPos.y = Players[0].transform.position.y + (Players[1].transform.position.y - Players[0].transform.position.y) / 2;

        }
        else if (cameraTarget == CameraTarget.PLAYER1)
        {

            CameraTargetPos.x = Players[0].transform.position.x;
            CameraTargetPos.y = Players[0].transform.position.y;

        }
        else if (cameraTarget == CameraTarget.PLAYER2)
        {

            CameraTargetPos.x = Players[1].transform.position.x;
            CameraTargetPos.y = Players[1].transform.position.y;

        }

        posFinal = new Vector3(CameraTargetPos.x, CameraTargetPos.y + yOffset, originalZ);
        posOriginal = cameraHolder.transform.position;

        if (posFinal != posOriginal)
        {
            deltaTime += Time.deltaTime;
            float percentage = deltaTime / timeTraveling;

            if (posFinal.y < minY)
            {
                posFinal.y = minY;
            }

            cameraHolder.transform.position = Vector3.Lerp(posOriginal, posFinal, percentage);

            if (deltaTime >= 1 || posOriginal == posFinal)
            {
                deltaTime = 0;
            }
        }
        
       
        //Camera Zoom
        float distanceBetweenPlayers = Vector3.Distance(Players[0].transform.position, Players[1].transform.position);

        if (distanceBetweenPlayers > MaxDistance) distanceBetweenPlayers = MaxDistance;
        if (distanceBetweenPlayers < MinDistance) distanceBetweenPlayers = MinDistance;

        
        lastZoom = distanceBetweenPlayers;

        if (autoZoom) mainCam.orthographicSize = distanceBetweenPlayers;
    }

    public IEnumerator CameraShake(float duration, float magnitude)
    {
        Vector3 originalPos = this.transform.position;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = UnityEngine.Random.Range(-1f, 1f) * magnitude;
            float y = UnityEngine.Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x,y,originalPos.z);


            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = Vector3.zero;

    }

    public IEnumerator CameraZoom(float duration, float zoom)
    {
        autoZoom = false;
        float originalZoom = mainCam.orthographicSize;

        float elapsed = 0.0f;
        float percentage = 0.0f;

        while (elapsed < duration)
        {
            percentage = elapsed / duration;
            Vector3 zoomLerp = Vector3.Lerp(new Vector3(originalZoom,0,0), new Vector3(zoom,0,0), percentage);

            mainCam.orthographicSize = zoomLerp.x;

            elapsed += Time.deltaTime;

            yield return null;
        }
    }

    public IEnumerator ResetCameraZoom()
    {

        float StartZoom = mainCam.orthographicSize;

        float elapsed = 0.0f;
        float percentage = 0.0f;

        while (elapsed < 2)
        {
            percentage = elapsed / 2;
            Vector3 zoomLerp = Vector3.Lerp(new Vector3(StartZoom, 0, 0), new Vector3(lastZoom, 0, 0), percentage);

            mainCam.orthographicSize = zoomLerp.x;

            elapsed += Time.deltaTime;

            yield return null;
        }

        autoZoom = true;
    }

}
