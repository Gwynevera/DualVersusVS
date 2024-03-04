using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingLine : MonoBehaviour
{
    [SerializeField] private Transform leftChunk;
    [SerializeField] private Transform rightChunk;

    [SerializeField] private float endForce = 100;

    private Vector2 leftPos;
    private Vector2 rightPos;

    private GameObject playerBreakingThrough;
    public float interpolationDuration = 1;

    private float elapsedTime = 0;
    private float percentageComplete = 0;

    [HideInInspector] public bool lerping = false;
    [HideInInspector] public bool rightToLeft = false;

    [Header("TEMPORAL")]
    [SerializeField] private Transform leftEnd;
    [SerializeField] private Transform rightEnd;

    private void Start()
    {
        leftPos = leftChunk.transform.position;
        rightPos = rightChunk.transform.position;
    }

    private void Update()
    {
        if (lerping)
        {
            playerBreakingThrough.GetComponent<Player>().enabled = false;
            playerBreakingThrough.GetComponent<Player>().knockback = true;
            playerBreakingThrough.GetComponent<Player>().breakingBuild = true;

            elapsedTime += Time.deltaTime;
            percentageComplete = elapsedTime / interpolationDuration;

            if (rightToLeft)
            {
                playerBreakingThrough.transform.position = Vector2.Lerp(rightPos, leftPos, percentageComplete);
            }
            else
            {
                playerBreakingThrough.transform.position = Vector2.Lerp(leftPos, rightPos, percentageComplete);
            }
            
            

            if (percentageComplete >= 1f) {

                endBreackThrough();
            }

            
        }
    }

    public void breakThroughBuilding(GameObject Player)
    {
        playerBreakingThrough = Player;
        lerping = true;
    }

    public void endBreackThrough()
    {
        playerBreakingThrough.GetComponent<Player>().enabled = true;
        lerping = false;

        Vector2 dir;

        if (rightToLeft) dir = new Vector2(-1, 0);
        else dir = new Vector2(1, 0);

        StartCoroutine(knockbakWaiter());

        //TEMPORAL PARA DEBUGAR

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if(playerBreakingThrough == players[0])
        {
            if (rightToLeft)
            {
                players[1].transform.position = leftEnd.position;
                players[0].transform.position = leftEnd.position + new Vector3(-15,0,0);
            }
            else
            {
                players[1].transform.position = rightEnd.position;
                players[0].transform.position = rightEnd.position + new Vector3(15, 0, 0);
            }
            
        }
        else
        {
            if (rightToLeft)
            {
                players[0].transform.position = leftEnd.position;
                players[1].transform.position = leftEnd.position + new Vector3(-15, 0, 0);
            }
            else
            {
                players[0].transform.position = rightEnd.position;
                players[1].transform.position = rightEnd.position + new Vector3(15, 0, 0);
            }
        }


    }

    public IEnumerator knockbakWaiter()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        playerBreakingThrough.GetComponent<Player>().knockback = false;
        playerBreakingThrough.GetComponent<Player>().breakingBuild = false;
    }

}
