using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingChunk : MonoBehaviour
{
    [SerializeField] private bool middleChunk = false;
    [SerializeField] private bool leftChunk = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Player" && collision.transform.GetComponent<Player>().knockback)
        {
            BuildingLine BL = GetComponentInParent<BuildingLine>();

            if (!middleChunk && !BL.lerping)
            {                
                if (leftChunk)
                {
                    BL.rightToLeft = false;
                }
                else
                {
                    BL.rightToLeft = true;
                }
                BL.breakThroughBuilding(collision.gameObject);
                Destroy(this.transform.gameObject);
            }
            else
            {
                Destroy(this.transform.gameObject);
            }
            
        }
    }
}
