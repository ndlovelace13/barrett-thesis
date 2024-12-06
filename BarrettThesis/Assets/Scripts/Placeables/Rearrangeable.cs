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
    protected virtual void Awake()
    {
        saveData = new Placeable();
        playerHand = GameObject.FindWithTag("ObjectSlot");
        DeactivateHighlight();
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
            Rearrangeable currentType = playerHand.GetComponentInChildren<Rearrangeable>();
            if (currentType != null && currentType.saveData.type == this.saveData.type)
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
        if (inPlace)
        {
            Debug.Log("Check Three");
            transform.SetParent(null);
            //transform.localScale = transform.localScale * 2f;
            GetComponent<ObjectMotion>().held = false;
            GetComponent<Collider>().enabled = true;
            saveData.SavePlacement(gameObject);
            SaveHandler.SaveSystem.SaveGame();
        }
    }

    public virtual void PlacementCheck()
    {
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 3f, surface))
            Place(hit);
        else
            Hold();
        
    }

    public virtual void Place(RaycastHit hit)
    {
        inPlace = true;
        GameObject currentWall = hit.collider.gameObject;
        transform.rotation = Quaternion.Euler(currentWall.transform.rotation.eulerAngles + new Vector3(0, 90, 0));
        transform.position = new Vector3(Round(hit.point.x), Round(hit.point.y), Round(hit.point.z)) + PlaceOffset(currentWall);
        
    }
    
    public virtual Vector3 PlaceOffset(GameObject wall)
    {
        //Debug.Log(wall.transform.forward);
        return Vector3.zero;
    }

    public virtual void Hold()
    {
        if (inPlace)
            transform.localPosition = Vector3.zero;
        inPlace = false;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    public virtual string GetPrompt()
    {
        return "Press E to Rearrange";
    }

    public virtual void ActivateHighlight()
    {
        GetComponent<Outline>().enabled = true;
    }

    public virtual void DeactivateHighlight()
    {
        GetComponent<Outline>().enabled = false;
    }

    private float Round(float startVal)
    {
        return Mathf.Round(startVal * 2) / 2;
    }
}
