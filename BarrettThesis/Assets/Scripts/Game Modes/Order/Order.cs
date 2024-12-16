using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Order : CoreGameMode, IInteractable
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        gameMode = GameMode.ORDERING;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void PostCameraShift()
    {
        base.PostCameraShift();
        //DEBUGGING
        TutorialOrder();
    }

    private void TutorialOrder()
    {
        Placeable donationJar = new Placeable(PlaceableType.Donation);
        GameController.SaveData.newOrders.Add(donationJar);
        Debug.Log("Tutorial Order Placed");
        SaveHandler.SaveSystem.SaveGame();
    }

    public override string GetPrompt()
    {
        return "Press E to Place an Order";
    }
}
