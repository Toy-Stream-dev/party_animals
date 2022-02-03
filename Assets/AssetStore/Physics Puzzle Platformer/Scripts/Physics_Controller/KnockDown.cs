//----------------------------
//---Physics Puzzle Platformer
//---© TFM™
//-------------------

using UnityEngine;

public class KnockDown : MonoBehaviour
{
    
    //Exposed Variable
    //////////////////
    
    public PlayerController Player_Controller;
    
    //----------------------------------------
    
    void OnCollisionEnter()
    {
        if(this.GetComponent<Rigidbody>().velocity.magnitude > 20)
        {
            Player_Controller.KnockDown();
        }
    }
}
