using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eraser : MonoBehaviour
{

    [SerializeField] Outline eraserOutline;

    [SerializeField] Create creationHandler;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        EraserToggle();
    }

    public void EraserToggle()
    {
        creationHandler.eraserToggled = !creationHandler.eraserToggled;
        if (creationHandler.eraserToggled)
        {
            eraserOutline.enabled = true;
        }
        else
        {
            eraserOutline.enabled = false;
        }
        creationHandler.ColorSelect();
    }
}
