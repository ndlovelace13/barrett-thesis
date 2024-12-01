using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Painting : Interactable
{
    Flashcard associatedCard;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //function to assign a card

    //interact to move it - snap to predefined locations

    public override string GetPrompt()
    {
        return "Press E to Rearrange";
    }
}
