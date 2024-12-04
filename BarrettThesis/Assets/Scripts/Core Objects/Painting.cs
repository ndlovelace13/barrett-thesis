using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Painting : Interactable
{
    public Flashcard associatedCard;

    [SerializeField] Image image;

    public Placeable saveData;

    public bool inPlace = true;
    // Start is called before the first frame update
    void Start()
    {
        saveData = new Placeable();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //function to assign a card

    //interact to move it - snap to predefined locations
    public override void Interact()
    {
        GameObject obj = GameObject.FindWithTag("ObjectSlot");
        if (obj.transform.childCount > 0)
        {
            if (obj.GetComponentInChildren<CardFill>() != null)
            {
                CardFill card = obj.GetComponentInChildren<CardFill>();
                associatedCard = card.GetFlashcard();
                card.transform.SetParent(null, false);
                card.gameObject.SetActive(false);
                AssignImage();
            }
            else if (obj.GetComponentInChildren<Painting>() != null)
            {
                Debug.Log("Swap Painting Here");
            }
            Debug.Log("Painting Interact Case Not Assigned");
        }
        else
        {
            base.Interact();

            transform.SetParent(obj.transform, false);
            Debug.Log(transform.parent.name);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.Euler(Vector3.zero);
            //transform.localScale = transform.localScale / 2f; 
            GetComponent<ObjectMotion>().held = true;
            GetComponent<Collider>().enabled = false;
            inPlace = false;
            //StartCoroutine(MatchingCam());
            GameObject.FindWithTag("Player").GetComponent<PlayerInteraction>().RearrangePainting(gameObject);
        }

    }

    public override void CancelInteract()
    {
        if (inPlace)
        {
            Debug.Log("Check Three");
            transform.SetParent(null);
            //transform.localScale = transform.localScale * 2f;
            GetComponent<ObjectMotion>().held = false;
            GetComponent<Collider>().enabled = true;
            SaveHandler.SaveSystem.SaveGame();
        }
        base.CancelInteract();
        
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
            return "Press E to Rearrange";
        }
        
    }

    public void AssignImage()
    {
        if (associatedCard != null)
        {
            string filePath = GameController.SaveData.currentDeck.mediaPath + associatedCard.fields[associatedCard.imgFields[0]];
            //Debug.Log(filePath);
            byte[] fileData = File.ReadAllBytes(filePath);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);
            image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
            SaveHandler.SaveSystem.SaveGame();
        }
    }

    public void RestoreData(Placeable placedData)
    {
        saveData = placedData;
        saveData.Restore(gameObject);
        associatedCard = GameController.SaveData.currentDeck.cards[saveData.cardIndex];
        AssignImage();
    }
}
