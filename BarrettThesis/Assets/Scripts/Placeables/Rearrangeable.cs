using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Rearrangeable : MonoBehaviour, IInteractable
{
    public bool highlight = false;

    public Placeable saveData;

    public bool inPlace = true;

    public GameObject playerHand;

    public LayerMask surface;
    // Start is called before the first frame update
    void Start()
    {
        playerHand = GameObject.FindWithTag("ObjectSlot");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void RestoreData(Placeable placedData)
    {
        saveData = placedData;
        saveData.Restore(gameObject);
    }

    public virtual void Interact()
    {
        if (playerHand.transform.childCount > 0)
        {
            if (playerHand.GetComponentInChildren<Rearrangeable>().saveData.type == saveData.type)
            {
                Debug.Log("SWAP OBJECTS, THEY ARE THE SAME TYPE");
            }
            else
                Debug.Log("Interact Case Not Handled");
        }
        else
        {
            Debug.Log("Interact called on Rearrangeable");
            transform.SetParent(playerHand.transform, false);
            Debug.Log(transform.parent.name);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.Euler(Vector3.zero);
            //transform.localScale = transform.localScale / 2f; 
            GetComponent<ObjectMotion>().held = true;
            GetComponent<Collider>().enabled = false;
            inPlace = false;
            GameObject.FindWithTag("Player").GetComponent<PlayerInteraction>().RearrangeObj(gameObject);
        }
    }

    public virtual void CancelInteract()
    {
        Debug.Log("Canceling Interact");
    }

    public virtual string GetPrompt()
    {
        return "Press E to Rearrange";
    }
}
