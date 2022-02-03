//----------------------------
//---Physics Puzzle Platformer
//---© TFM™
//-------------------

using UnityEngine;

public class CameraController : MonoBehaviour
{
    //Exposed Variables
    ///////////////////
    
    [SerializeField]
    public Transform player;
    
    [SerializeField]
    public Vector3 positionOffset;
    
    [SerializeField]
    public float distance = 10.0f, rotateSpeed = 5.0f, 
    smoothness = 0.25f, minAngle = -45.0f, maxAngle = -10.0f;
    
    
    //Hidden Variables
    //////////////////
    
    private Camera cam;
    private float currentX = 0.0f, currentY = 0.0f;


    //-------------------------------------------------------
    
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        cam = Camera.main;
    }

    void Update()
    {
        if(Input.GetKeyDown("escape"))
        {
            Application.Quit();
        }
    
        
        currentX = currentX + Input.GetAxis("Mouse X") * rotateSpeed;
        currentY = currentY + Input.GetAxis("Mouse Y") * rotateSpeed;

        currentY = Mathf.Clamp(currentY, minAngle, maxAngle);
    }

    void FixedUpdate()
    {
        Vector3 dir = new Vector3(0, 1, -distance);
        Quaternion rotation = Quaternion.Euler(-currentY, -currentX, 0);
        cam.transform.position = Vector3.Lerp (cam.transform.position, player.position + rotation * dir, smoothness);

        cam.transform.LookAt(player.position + positionOffset);
    }

}
