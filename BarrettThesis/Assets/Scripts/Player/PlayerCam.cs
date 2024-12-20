using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public Transform direction;

    public float xRotation = 0;
    public float yRotation = 0;


    // Start is called before the first frame update
    void Start()
    {
        //lock the cursor & hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        xRotation = 0f;
        yRotation = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.GameControl.gameMode == GameMode.DEFAULT)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            //get mouse input from the player
            float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
            float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

            yRotation += mouseX;
            xRotation -= mouseY;
            RotClamp();

        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        direction.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    private void RotClamp()
    {
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        if (yRotation < 0)
        {
            yRotation = 360 + yRotation;
        }
        else if (yRotation > 360)
        {
            yRotation = yRotation - 360;
        }
    }
}
