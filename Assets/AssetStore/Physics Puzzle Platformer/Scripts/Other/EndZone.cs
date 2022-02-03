//----------------------------
//---Physics Puzzle Platformer
//---© TFM™
//-------------------


using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndZone : MonoBehaviour
{
    //Exposed Variables
    ///////////////////
    
    [SerializeField]
    public EndTrigger endTrigger;
    
    [SerializeField]
    private Image fader;
    
    [SerializeField]
    private float fadeSpeed;
    
    
    //Hidden Variables
    //////////////////
    
    bool initiatedLoad, startFade;
    
    //----------------------------------------
    
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            startFade = true;
        }
    }
    
    void Update()
    {
        if(startFade && endTrigger.reachedTheEnd)
        {
            if(fader.color.a != 1)
            {
                var newAlpha = fader.color.a;
                newAlpha = Mathf.Lerp(newAlpha, 1, fadeSpeed);

                fader.color = new Color(fader.color.r, fader.color.g, fader.color.b, newAlpha);
            }
            
            if(fader.color.a > 0.95f)
            {
                if(!initiatedLoad)
                {
                    initiatedLoad = true;
                    SceneManager.LoadScene(0);
                    startFade = false;
                }
            }
        }
    }
}
