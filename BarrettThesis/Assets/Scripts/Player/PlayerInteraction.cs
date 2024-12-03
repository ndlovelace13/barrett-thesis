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

    LayerMask wallMask;
    public bool rearranging = false;
    GameObject heldPainting;
    // Start is called before the first frame update
    void Start()
    {
        layerMask = LayerMask.GetMask("Interactable");
        wallMask = LayerMask.GetMask("Wall");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isInteracting && GameController.GameControl.gameMode == GameMode.DEFAULT)
        {
            InteractCheck();
        }
        else
        {
            if (Input.GetKey(KeyCode.Escape) && currentInteractable != null && !rearranging)
            {
                currentInteractable.GetComponent<Interactable>().CancelInteract();
                isInteracting = false;
            }
        }

        if (rearranging)
            RearrangeCheck();
        UpdatePrompt();
    }

    private void InteractCheck()
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
        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            currentInteractable.GetComponent<Interactable>().Interact();
            currentInteractable.GetComponent<Interactable>().highlight = false;
            isInteracting = true;
        }
    }

    private void RearrangeCheck()
    {
        //if E pressed, place the painting and leave it there
        if (Input.GetKeyDown(KeyCode.E) && heldPainting.GetComponent<Painting>().inPlace)
        {
            //currentInteractable.GetComponent<Interactable>().Interact();
            //currentInteractable.GetComponent<Interactable>().highlight = false;
            heldPainting.transform.SetParent(null);
            isInteracting = false;
            Debug.Log("Painting Placed");
        }

        //create a raycast, highlight any interactables that are within range
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 3f, wallMask))
        {
            heldPainting.GetComponent<Painting>().inPlace = true;
            GameObject currentWall = hit.collider.gameObject;
            heldPainting.transform.rotation = Quaternion.Euler(currentWall.transform.rotation.eulerAngles + new Vector3(0, 90, 0));
            heldPainting.transform.position = new Vector3(Round(hit.point.x), Round(hit.point.y), Round(hit.point.z));
            Debug.Log(hit.point);
        }
        else
        {
            heldPainting.GetComponent<Painting>().inPlace = false;
            heldPainting.transform.localPosition = Vector3.zero;
            heldPainting.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }

        
    }

    private float Round(float startVal)
    {
        return Mathf.Round(startVal * 2) / 2;
    }

    public void RearrangePainting(GameObject painting)
    {
        rearranging = true;
        heldPainting = painting;
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
