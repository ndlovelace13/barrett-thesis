using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{

    public void Interact();

    public void CancelInteract();

    public string GetPrompt();

    public void ActivateHighlight(GameObject interactable)
    {
         interactable.GetComponent<Outline>().enabled = true;
    }

    public void DeactivateHighlight(GameObject interactable)
    {
        interactable.GetComponent<Outline>().enabled = false;
    }
}
