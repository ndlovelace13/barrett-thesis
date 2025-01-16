using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{

    public bool Interact();

    public bool CancelInteract();

    public string GetPrompt();

    public void ActivateHighlight();

    public void DeactivateHighlight();
}
