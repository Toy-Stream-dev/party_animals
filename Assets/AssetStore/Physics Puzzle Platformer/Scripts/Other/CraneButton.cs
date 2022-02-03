//----------------------------
//---Physics Puzzle Platformer
//---© TFM™
//-------------------


using UnityEngine;

public class CraneButton : MonoBehaviour
{
    //Exposed Variables
    ///////////////////
    
    [SerializeField]
    private Animator blueContainer;
    
    [SerializeField]
    private bool isReverseButton;
    
    [SerializeField]
    private AudioSource audioSource;
    
    //----------------------------------------
    
    void OnTriggerEnter(Collider other)
    {
        if(!isReverseButton)
        {
            if(other.gameObject.layer == LayerMask.NameToLayer("RagdollHands"))
            {
                if(Input.GetAxisRaw("Fire1") != 0 || Input.GetAxisRaw("Fire2") != 0)
                {
                    blueContainer.Play("MoveCrane");
                    
                    audioSource.Stop();
                    
                    if(!audioSource.isPlaying)
                    {
                        audioSource.Play();
                    }
                }
            }
        }
        
        else if(isReverseButton)
        {
            if(other.gameObject.layer == LayerMask.NameToLayer("RagdollHands"))
            {
                if(Input.GetAxisRaw("Fire1") != 0 || Input.GetAxisRaw("Fire2") != 0)
                {
                    blueContainer.Play("MoveCrane2");
                    
                    audioSource.Stop();
                    
                    if(!audioSource.isPlaying)
                    {
                        audioSource.Play();
                    }
                }
            }
        }
    }
    
}
