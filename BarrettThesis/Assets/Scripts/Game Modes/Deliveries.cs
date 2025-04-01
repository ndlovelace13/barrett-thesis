using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

public class Deliveries : CoreGameMode, IInteractable
{
    [SerializeField] PlaceableHandler allPlaceable;

    //bool mismatch = false;
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

    //jk
    /*
    public void Mismatch()
    {
        Rearrangeable[] allObjs = GameObject.FindObjectsOfType<Rearrangeable>();
        if (allObjs.Length == GameController.SaveData.placeables.Count)
            mismatch = false;
        else
            mismatch = true;
        Debug.Log(allObjs.Length + " lkasjdfldksj " + GameController.SaveData.placeables.Count);
    }*/

    //equip a new item from the ordered list
    private void UnboxDelivery()
    {
        if (GameController.SaveData.orderedPlaceables.Count > 0)
        {
            if (allPlaceable == null)
                allPlaceable = GameObject.FindFirstObjectByType<PlaceableHandler>();

            //remove the current placeable from the orders
            Placeable newOrder = GameController.SaveData.orderedPlaceables[0];
            GameController.SaveData.orderedPlaceables.Remove(newOrder);
            //GameController.SaveData.placeables.Add(newOrder);

            GameObject newObj = allPlaceable.RetrieveOrder(newOrder);
            newObj.GetComponent<Rearrangeable>().saveData = newOrder;
            newObj.GetComponent<IInteractable>().Interact();
            player.GetComponent<PlayerInteraction>().RearrangeObj(newObj);
            player.GetComponent<PlayerInteraction>().isInteracting = false;

            //increment global placeable count
            GameController.SaveData.placeableCount++;
        }
        /*
        else if (mismatch)
        {
            Rearrangeable[] allObjs = GameObject.FindObjectsOfType<Rearrangeable>();
            Placeable newOrder = null;
            for (int i = 0; i < GameController.SaveData.placeables.Count; i++)
            {
                bool isAssigned = false;
                for (int j = 0; j < allObjs.Length; j++)
                {
                    if (allObjs[j].saveData == GameController.SaveData.placeables[i])
                    {
                        isAssigned = true;
                        break;
                    }
                }
                if (!isAssigned)
                {
                    newOrder = GameController.SaveData.placeables[i];
                    break;
                }    
            }
            if (newOrder == null)
            {
                Debug.Log("FAILED TO FIND ORDER");
                return;
            }
            
            GameObject newObj = allPlaceable.RetrieveOrder(newOrder);
            newObj.GetComponent<Rearrangeable>().saveData = newOrder;
            newObj.GetComponent<IInteractable>().Interact();
            player.GetComponent<PlayerInteraction>().RearrangeObj(newObj);
            player.GetComponent<PlayerInteraction>().isInteracting = false;

            Mismatch();
        }*/
        

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
