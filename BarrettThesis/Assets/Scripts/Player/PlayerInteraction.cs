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

    public bool rearranging = false;
    public GameObject heldObj;
    // Start is called before the first frame update
    void Start()
    {
        layerMask = LayerMask.GetMask("Interactable");
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInteracting && GameController.GameControl.gameMode == GameMode.DEFAULT)
        {
            InteractCheck();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape) && currentInteractable != null && !rearranging)
            {
                currentInteractable.GetComponent<IInteractable>().CancelInteract();
                isInteracting = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E pressed");
            //if E pressed, call interact() on the interactable
            if (currentInteractable != null)
            {
                Debug.Log("other");
                bool interactSuccess = currentInteractable.GetComponent<IInteractable>().Interact();
                if (interactSuccess)
                {
                    currentInteractable.GetComponent<IInteractable>().DeactivateHighlight();
                    if (currentInteractable.GetComponent<ObjectMotion>() != null)
                        isInteracting = false;
                    else
                        isInteracting = true;
                }
                else
                    Debug.Log("Interaction Unsuccessful");
                
            }
            else if (rearranging && (currentInteractable == heldObj || currentInteractable == null))
            {
                Debug.Log("placing");
                ResetHeldObj();
            }
        }

        if (rearranging)
            heldObj.GetComponent<Rearrangeable>().PlacementCheck();
        UpdatePrompt();
    }

    private void InteractCheck()
    {
        //create a raycast, highlight any interactables that are within range
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 3f, layerMask))
        {
            currentInteractable = hit.collider.gameObject;
            currentInteractable.GetComponent<IInteractable>().ActivateHighlight();
            //Debug.Log("Intersecting an interactable!");
        }
        else
        {
            if (currentInteractable != null)
            {
                Deselect();
            }
        }
    }

    public void ResetHeldObj()
    {
        rearranging = false;
        heldObj.GetComponent<ObjectMotion>().held = false;
        if (heldObj.GetComponent<IInteractable>() != null)
            heldObj.GetComponent<IInteractable>().CancelInteract();
        heldObj = null;
    }

    /*private void RearrangeCheck()
    {

        //create a raycast, highlight any interactables that are within range
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 3f, heldObj.GetComponent<Rearrangeable>().surface))
        {
            heldObj.GetComponent<Rearrangeable>().inPlace = true;
            GameObject currentWall = hit.collider.gameObject;
            heldObj.transform.rotation = Quaternion.Euler(currentWall.transform.rotation.eulerAngles + new Vector3(0, 90, 0));
            heldObj.transform.position = new Vector3(Round(hit.point.x), Round(hit.point.y), Round(hit.point.z));
            //Debug.Log(hit.point);
        }
        else
        {
            if (heldObj.GetComponent<Rearrangeable>().inPlace)
                heldObj.transform.localPosition = Vector3.zero;
            heldObj.GetComponent<Rearrangeable>().inPlace = false;
            heldObj.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }

        
    }*/

    private float Round(float startVal)
    {
        return Mathf.Round(startVal * 2) / 2;
    }

    public void RearrangeObj(GameObject painting)
    {
        Debug.Log("Beginning Rearrange Mode");
        rearranging = true;
        heldObj = painting;
        isInteracting = false;
    }

    private void UpdatePrompt()
    {
        if (isInteracting || currentInteractable == null)
            prompt = "";
        else
            prompt = currentInteractable.GetComponent<IInteractable>().GetPrompt();
        interactUI.text = prompt;
    }

    private void Deselect()
    {
        currentInteractable.GetComponent<IInteractable>().DeactivateHighlight();
        currentInteractable = null;
    }
}
