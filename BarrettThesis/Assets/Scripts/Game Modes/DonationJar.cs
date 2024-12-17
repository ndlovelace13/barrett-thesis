using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonationJar : CoreGameMode, IInteractable
{

    // Update is called once per frame
    void Update()
    {
        
    }

    public override bool Interact()
    {
        if (GameController.SaveData.jarBalance > 0)
        {
            ClaimDonations();
            return false;
        }
        else
        {
            return false;
        }
    }

    //update the player's balance
    private void ClaimDonations()
    {
        GameController.SaveData.balance += GameController.SaveData.jarBalance;
        GameController.SaveData.jarBalance = 0;
        GameObject.FindWithTag("Checklist").GetComponent<ChecklistDisplay>().TaskUpdate();
        SaveHandler.SaveSystem.SaveGame();
    }

    public override string GetPrompt()
    {
        if (GameController.SaveData.jarBalance > 0)
            return "Press E to retrieve " + ((float)(GameController.SaveData.jarBalance / 100f)).ToString("C0");
        else
            return "No Donations to retrieve";


    }
}
