using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Props to Aqualood for the tutorial
public class Paint : MonoBehaviour
{
    public Camera cam;
    public int xPixels = 1024;
    public int yPixels = 1024;
    public int brushSize = 4;
    public Color brushColor;

    public Transform topLeftCorner;
    public Transform bottomRightCorner;
    public Transform cursor;

    public Material mat;

    public Texture2D createdPainting;

    Color[] colorMap;

    int currentX = 0;
    int currentY = 0;

    float xMult;
    float yMult;

    //missedFrameChecking
    bool pressedLastFrame = false;
    int prevX;
    int prevY;

    bool paintingEnabled = false;

    Flashcard currentCard;

    private void Start()
    {
        
    }

    private void Update()
    {
        

        if (paintingEnabled)
        {
            if (Input.GetMouseButton(0))
                CalculatePixel();
            else
                pressedLastFrame = false;

            if (Input.GetMouseButton(1))
                StopPainting();
            
        }
        else
        {
            //DEBUGGING
            if (Input.GetMouseButton(0))
            {
                StartPainting(GameController.SaveData.currentDeck.cards[0]);
            }
        }
        
    }

    private void CalculatePixel()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10f))
        {
            cursor.position = hit.point;
            currentX = (int)((cursor.position.x - topLeftCorner.position.x) * xMult);
            currentY = (int)((cursor.position.y - topLeftCorner.position.y) * yMult);
            DrawAtPoint();
        }
        else
            pressedLastFrame= false;
    }

    private void DrawAtPoint()
    {
        //Debug.Log(currentX + " " + currentY);
        if (pressedLastFrame && (prevX != currentX || prevY != currentY))
        {
            int dist = (int)Mathf.Sqrt((currentX - prevX) * (currentX - prevX) + (currentY - prevY) * (currentY - prevY));
            for (int i = 0; i <= dist; i++)
            {
                Draw((i * currentX + (dist - i) * prevX) / dist, (i * currentY + (dist - i) * prevY) / dist);
            }
        }
        else
           Draw(currentX, currentY);
        pressedLastFrame = true;
        prevX = currentX;
        prevY = currentY;
        SetTexture();
    }

    void Draw(int xPix, int yPix)
    {
        int x = xPix - brushSize + 1;
        int y = yPix - brushSize + 1;
        int maxX = xPix + brushSize - 1;
        int maxY = yPix + brushSize - 1;
        //Debug.Log(maxX - x);
        //Debug.Log(maxY - y);

        //check if all pixels are within bounds
        if (x < 0)
            x = 0;
        if (y < 0)
            y = 0;
        if (maxX >= xPixels)
            maxX = xPixels - 1;
        if (maxY >= yPixels) 
            maxY = yPixels - 1;

        //assign the affected pixels in the colorMap
        for (int i = x; i <= maxX; i++)
        {
            for(int j = y; j <= maxY; j++)
            {
                //we do a little radius checking
                if ((i - xPix) * (i - xPix) + (j - yPix) * (j - yPix) <= brushSize * brushSize)
                {
                    colorMap[i * yPixels + j] = brushColor; //replace the color of the right pixel in the color map
                }
            }
        }

    }

    void SetTexture()
    {
        createdPainting.SetPixels(colorMap);
        createdPainting.Apply();
    }

    void ResetPainting()
    {
        for (int i = 0; i < colorMap.Length; i++)
            colorMap[i] = Color.white;
        SetTexture();
    }

    //call this when create mode is activated
    public void StartPainting(Flashcard card)
    {
        paintingEnabled = true;
        GameController.GameControl.gameMode = GameMode.ARCHIVE;

        currentCard = card;

        cam = Camera.main;
        colorMap = new Color[xPixels * yPixels];

        if (currentCard.useCustom)
        {
            Debug.Log("painting loc: " + currentCard.customArt);
            createdPainting = SaveHandler.SaveSystem.GetPainting(currentCard.customArt);
        }
        else
        {
            createdPainting = new Texture2D(yPixels, xPixels, TextureFormat.RGBA32, false);
            createdPainting.filterMode = FilterMode.Point;
        }


        mat.mainTexture = createdPainting;

        //set the initial multipliers
        xMult = xPixels / (bottomRightCorner.position.x - topLeftCorner.position.x);
        yMult = yPixels / (bottomRightCorner.position.y - topLeftCorner.position.y);

        ResetPainting();
    }

    //call this when painting is complete, store it to files and to the associated flashcard
    public void StopPainting()
    {
        SaveHandler.SaveSystem.SavePainting(createdPainting, currentCard);
        paintingEnabled = false;
        GameController.GameControl.gameMode = GameMode.DEFAULT;
        SaveHandler.SaveSystem.SaveGame();
    }
}
