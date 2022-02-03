//----------------------------
//---Physics Puzzle Platformer
//---© TFM™
//-------------------


using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    //Exposed Variables
    ///////////////////
    
    [SerializeField]
    private Image fader;
    
    [SerializeField]
    private float fadeSpeed;
    
    //-----------------------------------------
    
    void Update()
    {
        if(fader.color.a != 0)
        {
            var newAlpha = fader.color.a;
            newAlpha = Mathf.Lerp(newAlpha, 0, fadeSpeed);
            
            fader.color = new Color(fader.color.r, fader.color.g, fader.color.b, newAlpha);
        }
    }
}
