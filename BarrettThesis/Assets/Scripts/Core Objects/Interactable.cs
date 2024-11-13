using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public bool highlight = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Interact()
    {
        Debug.Log("Interacting");
    }

    public virtual void CancelInteract()
    {
        Debug.Log("Canceling Interact");
    }

    public virtual string GetPrompt()
    {
        return "Press E to Interact";
    }

    public void FixedUpdate()
    {
        if (highlight)
            GetComponent<Outline>().enabled = true;
        else
            GetComponent<Outline>().enabled = false;
    }
}
