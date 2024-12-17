using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuseumOpen : CoreGameMode, IInteractable
{
    // Update is called once per frame
    void Update()
    {
        
    }

    public override string GetPrompt()
    {
        //TO DO - Only allow prompt and interact if tasks are completed
        return "Press E to Open Museum to the Public";
    }

    public override bool Interact()
    {
        //TO DO - Only allow interact if tasks are completed
        GameController.SaveData.museumOpen = true;
        DeckManager.DeckManage.TasksComplete();
        
        //TO DO - Flip the Open Sign
        return false;
    }
}
