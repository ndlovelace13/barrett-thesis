using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSelect : Paint
{
    public Color selectedCol = Color.black;

    [SerializeField] Create creationHandler;

    //[SerializeField] Texture2D colorWheel;
    // Start is called before the first frame update
    protected override void Start()
    {
        paintable = LayerMask.GetMask("colors");
        paintingEnabled = true;

        xMult = xPixels / (topLeftCorner.localPosition.x - bottomRightCorner.localPosition.x);
        yMult = yPixels / (topLeftCorner.localPosition.z - bottomRightCorner.localPosition.z);
    }

    //retrieve the color instead
    protected override void DrawAtPoint()
    {
        selectedCol = createdPainting.GetPixel(currentX, currentY);
        creationHandler.ColorSelect();
    }
}
