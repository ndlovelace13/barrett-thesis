using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Matching : CoreGameMode, IInteractable
{

    int newCardsCorrect;
    int cardsCorrect;
    float cardRatio;

    public int cardsPerRound = 5;
    bool newCard = false;
    List<Flashcard> flashcards;

    //pools
    [SerializeField] ObjectPool promptObjectPool;
    [SerializeField] ObjectPool answerObjectPool;

    [SerializeField] GameObject promptHolder;
    [SerializeField] GameObject answerHolder;

    List<GameObject> activePrompts;
    List<GameObject> activeAnswers;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        gameMode = GameMode.MATCHING;
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
    }

    public override void CancelInteract()
    {
        ExitCards();
        base.CancelInteract();
       
    }

    /*public override IEnumerator CameraShift()
    {
        base.CameraShift();
        StartCoroutine(MainGameplay());
        yield return null;
    }*/

    protected override void PostCameraShift()
    {
        StartCoroutine(MainGameplay());
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
        Debug.Log("Selecting Cards: " + GameController.SaveData.cardQueue.Count + " Review Cards | " + GameController.SaveData.newQueue.Count + " New Cards");
        float currentRatio = 0;
        if (GameController.SaveData.newQueue.Count > 0)
            currentRatio = GameController.SaveData.cardQueue.Count / GameController.SaveData.newQueue.Count;

        //store the new variables

        //if must decide which card to present
        if (GameController.SaveData.newQueue.Count > 0 && GameController.SaveData.cardQueue.Count > 0)
        {
            if (currentRatio >= cardRatio)
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
        else
        {
            Debug.Log("No Cards Remain to study");
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
            newPrompt.transform.position = Vector3.zero;
            newPrompt.GetComponent<MatchingPrompt>().CardAssign(flashcard);
            newPrompt.transform.SetParent(promptHolder.transform, false);
            activePrompts.Add(newPrompt);
            newPrompt.GetComponent<MatchingPrompt>().newCard = newCard;

            //activate the answer
            GameObject newAnswer = answerObjectPool.GetPooledObject();
            newAnswer.SetActive(true);
            newAnswer.transform.position = Vector3.zero;
            newAnswer.transform.rotation = Quaternion.Euler(0, 180, 0);
            newAnswer.GetComponent<MatchingAnswer>().CardAssign(flashcard);
            newAnswer.transform.SetParent(answerHolder.transform, false);
            activeAnswers.Add(newAnswer);
        }
        promptHolder.GetComponent<CardArrange>().SpaceCards();
        answerHolder.GetComponent<CardArrange>().SpaceCards();
        Debug.Log(flashcards.Count + " prompts and answers created");
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
        for (int i = 0; i < activeAnswers.Count; i++)
        {
            activePrompts[i].transform.SetParent(null);
            activePrompts[i].SetActive(false);
            activeAnswers[i].transform.SetParent(null);
            activeAnswers[i].SetActive(false);
        }
        flashcards.Clear();
    }

    IEnumerator MainGameplay()
    {
        flashcards = new List<Flashcard>();
        activePrompts = new List<GameObject>();
        activeAnswers = new List<GameObject>();
        if (GameController.SaveData.newQueue.Count > 0)
            cardRatio = (GameController.SaveData.newQueue.Count + GameController.SaveData.cardQueue.Count) / GameController.SaveData.newQueue.Count;
        Debug.Log("OG ratio: " + cardRatio);
        CardSelect();
        StartCoroutine(MatchCheck());
        yield return null;
    }

    IEnumerator MatchCheck()
    {
        while (GameController.GameControl.gameMode == GameMode.MATCHING)
        {
            if ((activePrompts.Count > 0) && (activeAnswers.Count > 0))
            {
                GameObject selectedPrompt = null;
                GameObject selectedAnswer = null;
                foreach (var prompt in activePrompts)
                {
                    if (prompt.GetComponent<MatchingPrompt>().selected)
                    {
                        selectedPrompt = prompt;
                        break;
                    }
                }
                foreach (var prompt in activeAnswers)
                {
                    if (prompt.GetComponent<MatchingAnswer>().selected)
                    {
                        selectedAnswer = prompt;
                        break;
                    }
                }
                if (selectedPrompt != null && selectedAnswer != null)
                {
                    CheckEquality(selectedPrompt.GetComponent<MatchingPrompt>(), selectedAnswer.GetComponent<MatchingAnswer>());
                    if (flashcards.Count <= 0)
                        CardSelect();
                    else
                    {
                        promptHolder.GetComponent<CardArrange>().SpaceCards();
                        answerHolder.GetComponent<CardArrange>().SpaceCards();
                    }
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private void CheckEquality(MatchingPrompt prompt, MatchingAnswer answer)
    {
        //deselect both cards
        prompt.Deselect();
        answer.Deselect();

        if (prompt.GetCardID() != answer.GetCardID())
        {

            //call the failure function for the prompt, deselect both cards
            prompt.Incorrect(answer.GetCardID());

            
        }
        else
        {

            //call the success function, kill both cards
            prompt.Correct();

            flashcards.Remove(prompt.GetFlashcard());
            activePrompts.Remove(prompt.gameObject);
            activeAnswers.Remove(answer.gameObject);

            prompt.transform.SetParent(null);
            answer.transform.SetParent(null);

            prompt.gameObject.SetActive(false);
            answer.gameObject.SetActive(false);

            //increment counters
            if (newCard)
                newCardsCorrect++;
            else
                cardsCorrect++;
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

    
