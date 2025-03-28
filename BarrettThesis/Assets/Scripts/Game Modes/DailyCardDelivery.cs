using System.Collections;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

public class DailyCardDelivery : CoreGameMode, IInteractable
{
    public override bool Interact()
    {
        if (!GameController.SaveData.cardsUnboxed)
        {
            UnboxShipment();
            return false;
        }
        else
            return false;
    }

    private void UnboxShipment()
    {
        StartCoroutine(CardShipment()); 
        if (GameController.SaveData.dayIndex == 1)
        {
            StartCoroutine(MoneyShipment(5000));
        }
        GameController.SaveData.cardsUnboxed = true;
        GameController.SaveData.unlockedCardCount += GameController.SaveData.newQueue.Count;
        GameObject.FindWithTag("Checklist").GetComponent<ChecklistDisplay>().TaskUpdate();
    }

    //spawn popup and maybe do a card explosion
    IEnumerator CardShipment()
    {
        //Card explosion lerp here


        GameObject newPopup = Instantiate(GameController.GameControl.popup, transform);
        newPopup.GetComponent<PopupBehavior>().NewPopup("+" + GameController.SaveData.newQueue.Count + " to Archives", Color.yellow);
        

        yield return null;
    }

    IEnumerator MoneyShipment(int amount)
    {
        //update balance
        GameController.SaveData.balance += amount;

        yield return new WaitForSeconds(1f);

        //spawn popup
        GameObject newPopup = Instantiate(GameController.GameControl.popup, transform);
        newPopup.GetComponent<PopupBehavior>().NewPopup("+" + ((float)(amount / 100f)).ToString("C2"), Color.green);
    }

    public override string GetPrompt()
    {
        if (!GameController.SaveData.cardsUnboxed)
            return "Press E to add shipment of new cards to the archives";
        else
            return "Shipment already unboxed";
    }
}
