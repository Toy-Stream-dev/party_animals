//----------------------------
//---Physics Puzzle Platformer
//---© TFM™
//-------------------


using UnityEngine;

public class EndTrigger : MonoBehaviour
{
    //Exposed Variables
    ///////////////////
    
    [SerializeField]
    public bool reachedTheEnd;
    
    //----------------------------------------
    
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            reachedTheEnd = true;
        }
    }
}

