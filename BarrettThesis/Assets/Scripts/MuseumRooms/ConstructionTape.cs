using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionTape : MonoBehaviour, IInteractable
{
    public int constructionRoomIndex;

    public MuseumLoader museumControl;

    Outline[] outlines;

    //[SerializeField] GameObject nonTapedDoor;
    // Start is called before the first frame update
    public bool Interact()
    {
        //check whether the room is ready
        if (GameController.SaveData.roomData[constructionRoomIndex].dayFinished <= GameController.SaveData.dayIndex)
        {
            Debug.Log("Room finished, opening room access");
            GameController.SaveData.roomData[constructionRoomIndex].FinishConstruction();

            //update all doorways
            museumControl.AllDoorsUpdate();
        }
        //room is not completed with construction yet
        else
        {
            Debug.Log("Room not finished");
        }
        return false;
    }

    public bool CancelInteract()
    {
        return false;
    }

    public string GetPrompt()
    {
        return "Area still under construction";
    }

    public void ActivateHighlight()
    {
        foreach (Outline outline in outlines)
            outline.enabled = true;
    }

    public void DeactivateHighlight()
    {
        foreach (Outline outline in outlines)
            outline.enabled = false;
    }

    //called whenever a construction door is spawned
    public void AssignData(int linkedConstructionRoom)
    {
        museumControl = GameObject.FindObjectOfType<MuseumLoader>();
        constructionRoomIndex = linkedConstructionRoom;

        outlines = GetComponentsInChildren<Outline>();
    }
}
