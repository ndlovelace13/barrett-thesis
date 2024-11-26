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
    bool newCard = false;
    List<Flashcard> flashcards;

    //pools
    [SerializeField] ObjectPool promptObjectPool;
    [SerializeField] ObjectPool answerObjectPool;

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
        ExitCards();
        base.CancelInteract();
        GameController.GameControl.lockPlayer = false;
        GameController.GameControl.gameMode = GameMode.DEFAULT;
       
    }

    public override string GetPrompt()
    {
        if (GameController.SaveData.cardQueue.Count > 0 || GameController.SaveData.newQueue.Count > 0)
            return "Press E to Start Matching";
        else
            return "Press E for Extra Practice";
    }

    public void CardSelect()
    {
        Debug.Log("Selecting Cards");
        cardRatio = GameController.SaveData.cardQueue.Count / GameController.SaveData.newQueue.Count;
        //store the new variables

        //if must decide which card to present
        if (GameController.SaveData.newQueue.Count > 0 && GameController.SaveData.cardQueue.Count > 0)
        {
            if (cardsCorrect > newCardsCorrect * GameController.SaveData.newQueue.Count)
                flashcards = SelectQueue();
            else
                flashcards = NewQueue();
        }
        //if no old cards remain
        else if (GameController.SaveData.cardQueue.Count == 0 && GameController.SaveData.newQueue.Count > 0)
        {
            flashcards = NewQueue();
        }
        //if no new cards remain
        else if (GameController.SaveData.cardQueue.Count > 0)
        {
            flashcards = SelectQueue();
        }
        Debug.Log(flashcards.Count + " cards selected");
        ObjectCreation();
    }

    public void ObjectCreation()
    {
        foreach (Flashcard flashcard in flashcards)
        {
            //activate the prompt
            GameObject newPrompt = promptObjectPool.GetPooledObject();
            newPrompt.SetActive(true);
            newPrompt.GetComponent<MatchingPrompt>().ActivatePrompt(flashcard);

            //activate the answer
            GameObject newAnswer = answerObjectPool.GetPooledObject();
            newAnswer.SetActive(true);
            newAnswer.GetComponent<MatchingAnswer>().ActivateAnswer(flashcard);
        }
    }

    public void ExitCards()
    {
        Debug.Log("Replacing Cards");
        if (flashcards.Count > 0)
        {
            if (newCard)
            {
                GameController.SaveData.newQueue.Insert(0, flashcards[0]);
            }
            else
            {
                GameController.SaveData.cardQueue = GameController.SaveData.cardQueue.Union(flashcards).ToList();
            }
        }
        flashcards.Clear();
    }

    IEnumerator MainGameplay()
    {
        flashcards = new List<Flashcard>();
        while (GameController.GameControl.gameMode == GameMode.MATCHING)
        {
            if (flashcards.Count == 0)
            {
                //fill the wall with new cards
                CardSelect();
            }
            else
            {
                //check for matches
            }
            yield return new WaitForEndOfFrame();
        }
        
    }

    private List<Flashcard> NewQueue()
    {
        List<Flashcard> selectedCards = new List<Flashcard>();
        selectedCards.Add(GameController.SaveData.newQueue[0]);
        GameController.SaveData.newQueue.RemoveAt(0);
        newCard = true;
        return selectedCards;
    }

    private List<Flashcard> SelectQueue()
    {
        List<Flashcard> selectedCards = new List<Flashcard>();
        int chosen = 0;
        int index = 0;

        while (GameController.SaveData.cardQueue.Count > 0 && chosen < cardsPerRound)
        {
            index = Random.Range(0, GameController.SaveData.cardQueue.Count);
            Flashcard selected = GameController.SaveData.cardQueue[index];
            selectedCards.Add(selected);
            GameController.SaveData.cardQueue.RemoveAt(index);
            chosen++;
        }
        newCard = false;
        return selectedCards;
    }
}

    
