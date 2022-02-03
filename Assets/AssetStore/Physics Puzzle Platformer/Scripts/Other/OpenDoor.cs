//----------------------------
//---Physics Puzzle Platformer
//---© TFM™
//-------------------


using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    //Exposed Variables
    ///////////////////
    
    [SerializeField]
    private BlockTrigger trigger1, trigger2, trigger3;
    
    [SerializeField]
    private Animator door;
    
    [SerializeField]
    private AudioSource audioSource;
    
    
    //Hidden Variables
    ///////////////////
    
    bool opened;
    
    //-----------------------------------------
    
    void Update()
    {
        if(!opened && trigger1.block1 && trigger2.block2 && trigger3.block3)
        {
            opened = true;
            door.SetFloat("DoorSpeed", 1);
            door.Play("Open");
            
            audioSource.Stop();
            
            if(!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        
        else if(opened && (!trigger1.block1 || !trigger2.block2 || !trigger3.block3))
        {
            opened = false;
            door.SetFloat("DoorSpeed", -1);
            door.Play("Open");
            
            audioSource.Stop();
            
            if(!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }
}
