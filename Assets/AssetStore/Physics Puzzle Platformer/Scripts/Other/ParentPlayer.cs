//----------------------------
//---Physics Puzzle Platformer
//---© TFM™
//-------------------


using UnityEngine;

public class ParentPlayer : MonoBehaviour
{
    //Exposed Variables
    ///////////////////
    
    [SerializeField]
    private GameObject player;
    
    //-----------------------------------------
    
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            player.transform.parent = this.gameObject.transform;
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            player.transform.parent = null;
        }
    }
}
