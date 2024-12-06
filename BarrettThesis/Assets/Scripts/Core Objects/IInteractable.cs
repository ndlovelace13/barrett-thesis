using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{

    public void Interact();

    public void CancelInteract();

    public string GetPrompt();

    public void ActivateHighlight();

    public void DeactivateHighlight();
}
