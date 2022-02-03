//----------------------------
//---Physics Puzzle Platformer
//---© TFM™
//-------------------


using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Buttons_animation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    //Exposed Variables
    ///////////////////
    
    [SerializeField]
    private Sprite normal, hover;
    
    [SerializeField]
    private Image sourceImg;
    
    [SerializeField]
    private Animator thisAnimator;
    
    [SerializeField]
    private string idleAnimation, hoverAnimation;
    
    [SerializeField]
    private AudioSource audioSource;
    
    [SerializeField]
    private AudioClip hoverSound, popSound;
    
    
    //Hidden Variables
    //////////////////
    
    private int animLayer = 0;

    
    //---------------------------------------------
    
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        sourceImg.sprite = hover;
        
        if(!isPlaying(thisAnimator, hoverAnimation) && !isPlaying(thisAnimator, idleAnimation))
        {
                thisAnimator.Play(hoverAnimation);
                
                if(!audioSource.isPlaying)
                {
                    audioSource.clip = hoverSound;
                    audioSource.Play();
                }
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        sourceImg.sprite = normal;
        
        if(!isPlaying(thisAnimator, idleAnimation) && !isPlaying(thisAnimator, hoverAnimation))
        {
                thisAnimator.Play(idleAnimation);
        }
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        sourceImg.sprite = normal;
        
        if(!audioSource.isPlaying)
        {
            audioSource.clip = popSound;
            audioSource.Play();
        }
    }
    
    bool isPlaying(Animator anim, string stateName)
    {
        if (anim.GetCurrentAnimatorStateInfo(animLayer).IsName(stateName) && anim.GetCurrentAnimatorStateInfo(animLayer).normalizedTime < 1.0f)
        {
            return true;
        }
        
        else
        {
            return false;
        }
    }
}
