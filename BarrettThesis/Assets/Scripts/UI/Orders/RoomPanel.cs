using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoomPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public RoomData currentRoom;

    Image sprite;

    bool fading = true;
    bool colorOverride = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*if (currentRoom.tempRoom)
        {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0.5f);
        }*/
    }

    public void AssignRoom(RoomData newRoom)
    {
        currentRoom = newRoom;
        sprite = GetComponent<Image>();
        StartCoroutine(RoomFlash());
    }

    IEnumerator RoomFlash()
    {
        if (currentRoom.tempRoom)
        {
            while (gameObject.activeSelf)
            {
                float currentAlpha = sprite.color.a;

                //check for a switch
                if (sprite.color.a <= 0f)
                {
                    fading = false;
                }
                else if (sprite.color.a >= 1f)
                {
                    fading = true;
                }

                //adjust the current alpha
                if (fading)
                    currentAlpha -= 0.001f;
                else
                    currentAlpha += 0.001f;

                //set the color
                if (!colorOverride)
                    sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, currentAlpha);
                else
                    sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1f);

                yield return new WaitForEndOfFrame();
            }
        }
        yield return null;
    }

    public void OnPointerEnter(PointerEventData dat)
    {
        colorOverride = true;
    }

    public void OnPointerExit(PointerEventData dat)
    {
        colorOverride = false;
    }

    public void OnPointerDown(PointerEventData dat)
    {
        RoomPurchase();
    }

    public void RoomPurchase()
    {
        Debug.Log("Room Purchased");
    }
}
