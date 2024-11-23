using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Matching : Interactable
{
    int newCardsCorrect;
    int cardsCorrect;
    int cardRatio;

    public int cardsPerRound = 5;

    // Start is called before the first frame update
    void Start()
    {
        cardsCorrect = 0;
        newCardsCorrect = 0;
        cardRatio = 0;
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
        if (GameController.SaveData.cardQueue.Count > 0)
            return "Press E to Start Matching";
        else
            return "Press E for Extra Practice";
    }

    IEnumerator MainGameplay()
    {
        cardRatio = GameController.SaveData.cardQueue.Count / GameController.SaveData.newQueue.Count;
        //store the new variables

        List<Flashcard> flashcards = new List<Flashcard>();
        //if must decide which card to present
        if (GameController.SaveData.newQueue.Count > 0 && GameController.SaveData.cardQueue.Count > 0)
        {
            if (cardsCorrect > newCardsCorrect * GameController.SaveData.newQueue.Count)
                SelectQueue();
            else
                NewQueue();
        }
        //if no old cards remain
        else if (GameController.SaveData.cardQueue.Count == 0 && GameController.SaveData.newQueue.Count > 0)
        {
            NewQueue();
        }
        //if no new cards remain
        else if (GameController.SaveData.cardQueue.Count > 0)
        {
            SelectQueue();
        }

        
        //randomly select cards from current queue
        yield return null;
    }

    private List<Flashcard> NewQueue()
    {
        List<Flashcard> selectedCards = new List<Flashcard>();
        selectedCards.Add(GameController.SaveData.newQueue[0]);
        GameController.SaveData.newQueue.RemoveAt(0);
        return selectedCards;
    }

    private List<Flashcard> SelectQueue()
    {
        List<Flashcard> selectedCards = new List<Flashcard>();
        int chosen = 0;
        int index = 0;

        while (GameController.SaveData.cardQueue.Count > 0 && chosen <  cardsPerRound)
        {
            index = Random.Range(0, GameController.SaveData.cardQueue.Count);
            Flashcard selected = GameController.SaveData.cardQueue[index];
            selectedCards.Add(selected);
            GameController.SaveData.cardQueue.RemoveAt(index);
            chosen++;
        }

        return selectedCards;
    }

    private void 
}

    
