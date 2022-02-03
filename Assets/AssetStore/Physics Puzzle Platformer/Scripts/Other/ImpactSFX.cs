//----------------------------
//---Physics Puzzle Platformer
//---© TFM™
//-------------------


using UnityEngine;

public class ImpactSFX : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;
    
    [SerializeField]
    private AudioClip[] clips;
    
    AudioClip chosenClip;
    
    //----------------------------------------
    
    void OnCollisionEnter(Collision col)
    {
        if(!audioSource.isPlaying && col.gameObject.layer == LayerMask.NameToLayer("World"))
        {
            chosenClip = clips[Random.Range(0, clips.Length)];
            audioSource.clip = chosenClip;
            audioSource.Play();
        }
    }
}
