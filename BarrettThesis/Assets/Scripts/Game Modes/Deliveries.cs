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

    //update the player's balance
    private void UnboxDelivery()
    {
        Placeable newOrder = GameController.SaveData.orderedPlaceables[0];
        GameController.SaveData.orderedPlaceables.Remove(newOrder);
        GameObject newObj = allPlaceable.RetrieveOrder(newOrder);
        newObj.GetComponent<IInteractable>().Interact();
        player.GetComponent<PlayerInteraction>().RearrangeObj(newObj);
        player.GetComponent<PlayerInteraction>().isInteracting = false;
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
