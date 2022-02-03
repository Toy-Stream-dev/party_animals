//----------------------------
//---Physics Puzzle Platformer
//---© TFM™
//-------------------


using UnityEngine;
using UnityEngine.SceneManagement;

public class Navigation : MonoBehaviour
{
    //Exposed Variables
    ///////////////////
    
    [SerializeField]
    private GameObject menu, about;
    
    
    
    //-------------------------------------
    
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    
    public void showMenu()
    {
        if(!menu.activeSelf)
        {
            menu.SetActive(true);
            
            if(about.activeSelf)
            {
                about.SetActive(false);
            }
        }
    }
    
    public void LoadDemoGame()
    {
        SceneManager.LoadScene(1);
    }
    
    public void showAbout()
    {
        if(!about.activeSelf)
        {
            if(menu.activeSelf)
            {
                menu.SetActive(false);
            }
            
            about.SetActive(true);
        }
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }
}
