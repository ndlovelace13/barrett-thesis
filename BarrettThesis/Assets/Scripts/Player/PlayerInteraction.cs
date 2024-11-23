using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    LayerMask layerMask;
    GameObject currentInteractable;
    public TMP_Text interactUI;

    string prompt;
    public bool isInteracting = false;
    // Start is called before the first frame update
    void Start()
    {
        layerMask = LayerMask.GetMask("Interactable");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isInteracting && GameController.GameControl.gameMode == GameMode.DEFAULT)
        {
            //create a raycast, highlight any interactables that are within range
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 3f, layerMask))
            {
                currentInteractable = hit.collider.gameObject;
                currentInteractable.GetComponent<Interactable>().highlight = true;
                //Debug.Log("Intersecting an interactable!");
            }
            else
            {
                if (currentInteractable != null)
                {
                    Deselect();
                }
            }

            //if E pressed, call interact() on the interactable
            if (Input.GetKey(KeyCode.E) && currentInteractable != null)
            {
                currentInteractable.GetComponent<Interactable>().Interact();
                currentInteractable.GetComponent<Interactable>().highlight = false;
                isInteracting = true;
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.Escape) && currentInteractable != null)
            {
                currentInteractable.GetComponent<Interactable>().CancelInteract();
                isInteracting = false;
            }
        }
        UpdatePrompt();
    }

    private void UpdatePrompt()
    {
        if (isInteracting || currentInteractable == null)
            prompt = "";
        else
            prompt = currentInteractable.GetComponent<Interactable>().GetPrompt();
        interactUI.text = prompt;
    }

    private void Deselect()
    {
        currentInteractable.GetComponent<Interactable>().highlight = false;
        currentInteractable = null;
    }
}
