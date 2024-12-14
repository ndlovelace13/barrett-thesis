using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brush : MonoBehaviour
{

    [SerializeField] int brushSize;

    [SerializeField] Create creationHandler;

    [SerializeField] Outline brushOutline;
    //bool selected;
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
        Select();
    }

    public void Select()
    {
        //selected = true;
        creationHandler.BrushSelect(this);
        brushOutline.enabled = true;
    }

    public void Deselect()
    {
        //selected = false;
        brushOutline.enabled = false;
    }

    public int GetSize()
    {
        return brushSize;
    }
}
