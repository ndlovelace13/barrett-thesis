using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Matching : Interactable
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Interact()
    {
        base.Interact();
        GameController.GameControl.lockPlayer = true;
        GameController.GameControl.gameMode = GameMode.MATCHING;
        StartCoroutine(MainGameplay());
    }

    public override void CancelInteract()
    {
        base.CancelInteract();
        GameController.GameControl.lockPlayer = false;
        GameController.GameControl.gameMode = GameMode.DEFAULT;
       
    }

    public override string GetPrompt()
    {
        if (DeckManager.DeckManage.cardQueue.Count > 0)
            return "Press E to Start Matching";
        else
            return "Press E for Extra Practice";
    }

    IEnumerator MainGameplay()
    {
        //randomly select cards from current queue
        yield return null;
    }
}
