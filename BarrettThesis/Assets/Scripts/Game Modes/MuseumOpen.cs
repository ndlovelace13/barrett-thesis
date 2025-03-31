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
        if (GameController.SaveData.tasksComplete || GameController.GameControl.testingMode)
            return "Press E to Open Museum to the Public";
        else
            return "The Museum is Not Ready for Visitors!";
    }

    public override bool Interact()
    {
        //TO DO - Only allow interact if tasks are completed
        if (GameController.SaveData.tasksComplete || GameController.GameControl.testingMode)
        {
            GameController.SaveData.museumOpen = true;
            DeckManager.DeckManage.TasksComplete();

            GameObject.FindFirstObjectByType<TutorialControl>().CheckTutorial("firstDayFinish");

            //reset the navmesh because plot reasons
            GameObject.FindFirstObjectByType<MuseumLoader>().RebuildNav();
        }
        return false;
    }
}
