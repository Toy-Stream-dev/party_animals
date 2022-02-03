//----------------------------
//---Physics Puzzle Platformer
//---© TFM™
//-------------------


using UnityEngine;

public class DunkTrigger : MonoBehaviour
{
    //Exposed Variables
    ///////////////////
    
    [SerializeField]
    private Animator slideDoor;
    
    [SerializeField]
    private AudioSource audioSource;
    
    //---------------------------------------
    
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Ball"))
        {
            slideDoor.Play("slideOpen");
            audioSource.Play();
        }
    }
}
