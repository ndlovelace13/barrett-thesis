using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Unity.VisualScripting;

public class Painting : Rearrangeable, IInteractable
{
    public Flashcard associatedCard;

    [SerializeField] Image image;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        surface = LayerMask.GetMask("Wall");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //function to assign a card

    //interact to move it - snap to predefined locations
    public override void Interact()
    {
        base.Interact();
        Debug.Log("Check 1");
        if (playerHand.transform.childCount > 0)
        {
            if (playerHand.GetComponentInChildren<CardFill>() != null)
            {
                Debug.Log("Check 2");
                CardFill card = playerHand.GetComponentInChildren<CardFill>();
                associatedCard = card.GetFlashcard();
                card.transform.SetParent(null, false);
                card.gameObject.SetActive(false);
                AssignImage(true);
            }
        }

    }

    public override void CancelInteract()
    {
        base.CancelInteract();
        Debug.Log("Painting Placed");
    }

    public override string GetPrompt()
    {
        GameObject obj = GameObject.FindWithTag("ObjectSlot");
        if (obj != null)
        {
            if (obj.GetComponentInChildren<CardFill>() != null)
            {
                return "Press E to Assign";
            }
            else if (transform.parent == obj)
            {
                return "Press E to Place Painting";
            }
            else
            {
                return "Press E to Replace";
            }
        }
        else
        {
            return base.GetPrompt();
        }
        
    }

    public void AssignImage(bool save)
    {
        if (associatedCard != null)
        {
            string filePath = GameController.SaveData.currentDeck.mediaPath + associatedCard.fields[associatedCard.imgFields[0]];
            //Debug.Log(filePath);
            byte[] fileData = File.ReadAllBytes(filePath);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);
            image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
            if (save)
            {
                saveData.SavePlacement(gameObject);
                SaveHandler.SaveSystem.SaveGame();
            }
                
        }
    }

    public override void RestoreData(Placeable placedData)
    {
        base.RestoreData(placedData);
        associatedCard = GameController.SaveData.currentDeck.cards[saveData.cardIndex];
        AssignImage(false);
    }
}
