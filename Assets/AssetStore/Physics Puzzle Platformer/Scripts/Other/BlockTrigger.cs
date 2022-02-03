//----------------------------
//---Physics Puzzle Platformer
//---© TFM™
//-------------------


using UnityEngine;

public class BlockTrigger : MonoBehaviour
{
    //Exposed Variables
    ///////////////////
    
    [SerializeField]
    private int triggerNumber;
    
    //Hidden Variables
    ///////////////////
    
    [HideInInspector]
    public bool block1, block2, block3;
    
    void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<blockType>())
        {
            if(other.GetComponent<blockType>().blockNumber == 1 && triggerNumber == 1)
            {
                block1 = true;
            }

            if(other.GetComponent<blockType>().blockNumber == 2 && triggerNumber == 2)
            {
                block2 = true;
            }

            if(other.GetComponent<blockType>().blockNumber == 3 && triggerNumber == 3)
            {
                block3 = true;
            }
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<blockType>())
        {
            if(other.GetComponent<blockType>().blockNumber == 1 && triggerNumber == 1)
            {
                block1 = false;
            }

            if(other.GetComponent<blockType>().blockNumber == 2 && triggerNumber == 2)
            {
                block2 = false;
            }

            if(other.GetComponent<blockType>().blockNumber == 3 && triggerNumber == 3)
            {
                block3 = false;
            }
        }
    }
}
