//----------------------------
//---Physics Puzzle Platformer
//---© TFM™
//-------------------

using UnityEngine;

public class BalanceController : MonoBehaviour
{
    //Exposed Variables
    ///////////////////
    
    [SerializeField]
    private PlayerController controller;
    
    //----------------------------------------
    
    void OnCollisionEnter(Collision col)
    {
        controller.PlayerGetUp();
    }
}
