using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deliveries : CoreGameMode, IInteractable
{
    [SerializeField] PlaceableHandler allPlaceable;
    public override bool Interact()
    {
        if (GameController.SaveData.orderedPlaceables.Count > 0)
        {
            UnboxDelivery();
            return false;
        }
        else
        {
            return false;
        }
    }

    //equip a new item from the ordered list
    private void UnboxDelivery()
    {
        Placeable newOrder = GameController.SaveData.orderedPlaceables[0];
        GameController.SaveData.orderedPlaceables.Remove(newOrder);
        GameObject newObj = allPlaceable.RetrieveOrder(newOrder);
        newObj.GetComponent<IInteractable>().Interact();
        player.GetComponent<PlayerInteraction>().RearrangeObj(newObj);
        player.GetComponent<PlayerInteraction>().isInteracting = false;
        //update the checklist on unbox
        GameObject.FindWithTag("Checklist").GetComponent<ChecklistDisplay>().TaskUpdate();
    }

    private void SpawnNewObj()
    {
        
    }

    public override string GetPrompt()
    {
        if (GameController.SaveData.orderedPlaceables.Count > 0)
            return "Press E to unbox a delivery";
        else
            return "All deliveries unboxed";


    }
}
