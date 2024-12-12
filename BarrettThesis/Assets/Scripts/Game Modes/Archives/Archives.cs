using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archives : CoreGameMode, IInteractable
{
    ObjectPool cardPool;
    public List<GameObject> displayedCards;

    //cam control
    //public Transform camControl;

    int currentIndex;
    int indexModAmount = 1;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        cardPool = GameObject.FindWithTag("CardPool").GetComponent<ObjectPool>();
        displayedCards = new List<GameObject>();
        gameMode = GameMode.ARCHIVE;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.GameControl.gameMode == GameMode.ARCHIVE)
        {
            //Debug.Log("Still in Archive???");
            //get archive increment
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                cardIncrement(-indexModAmount);
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                cardIncrement(indexModAmount);

            //retrieve card for placement in painting
            if (Input.GetKeyDown(KeyCode.Q))
            {
                CardRetrieve();
            }
        }

        
    }

    public override bool Interact()
    {
        currentIndex = 0;
        return base.Interact();
        //StartCoroutine(ArchiveCam());
    }

    public override void CancelInteract()
    {
        base.CancelInteract();
        //camControl.rotation = Camera.main.transform.rotation;
        StartCoroutine(RemoveCard(displayedCards));
    }

    public void CardRetrieve()
    {
        if (GameObject.FindWithTag("ObjectSlot").GetComponentInChildren<CardFill>() != null)
        {
            List<GameObject> heldCard = new List<GameObject>();
            GameObject handCard = GameObject.FindWithTag("ObjectSlot").GetComponentInChildren<CardFill>().gameObject;
            heldCard.Add(handCard);
            StartCoroutine(RemoveCard(heldCard));
            handCard.GetComponent<CardFill>().StopHolding();
        }
        GameObject currentCard = displayedCards[0];
        displayedCards.Remove(currentCard);

        //place card in hand
        currentCard.GetComponent<CardFill>().StartHolding();

        //set rules of interacting
        player.GetComponent<PlayerInteraction>().heldObj = currentCard;
        player.GetComponent<PlayerInteraction>().isInteracting = false;
        CancelInteract();
    }

    public override string GetPrompt()
    {
        return "Press E to Check the Archives";
    }

    public void cardIncrement(int indexMod)
    {
        //check whether index is within bounds
        int newTempIndex = currentIndex + indexMod;
        if (newTempIndex >= 0 && newTempIndex < GameController.SaveData.currentDeck.cards.Count)
        {
            //disable currentCards
            if (displayedCards.Count > 0)
                StartCoroutine(RemoveCard(displayedCards));

            currentIndex = newTempIndex;
            StartCoroutine(SpawnCard(currentIndex, indexModAmount));
        }
    }

    /*public override IEnumerator CameraShift()
    {
        base.CameraShift();
        StartCoroutine(SpawnCard(currentIndex, indexModAmount));
        yield return null;
    }*/

    protected override void PostCameraShift()
    {
        StartCoroutine(SpawnCard(currentIndex, indexModAmount));
    }

    IEnumerator SpawnCard(int cardIndex, int cardNum)
    {
        List<GameObject> newCards = new List<GameObject>();
        for (int i = 0; i < cardNum; i++)
        {
            GameObject currentCard = cardPool.GetPooledObject();
            currentCard.SetActive(true);

            //set card a certain dist from the user
            currentCard.transform.position = Camera.main.transform.position + Camera.main.transform.forward;
            currentCard.GetComponent<Rigidbody>().useGravity = false;
            currentCard.GetComponent<Rigidbody>().velocity = Vector3.zero;
            currentCard.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            //currentCard.GetComponent<BoxCollider>().enabled = false;

            //in case need to set rotation
            currentCard.transform.LookAt(Camera.main.transform);
            //currentCard.transform.rotation = currentCam;

            //fill the card
            Flashcard currentFlashcard = GameController.SaveData.currentDeck.cards[cardIndex + i];
            currentCard.GetComponent<CardFill>().CardAssign(currentFlashcard);
            currentCard.GetComponent<CardMotion>().selected = true;

            //add the new card to the newCards list
            newCards.Add(currentCard);

            yield return new WaitForFixedUpdate();
        }

        displayedCards = newCards;
        Debug.Log("Displayed Cards: " +  displayedCards.Count);
        yield return null;
    }

    IEnumerator RemoveCard(List<GameObject> currentCards)
    {
        foreach (GameObject card in currentCards)
        {
            Debug.Log("Removing Card");
            card.GetComponent<Rigidbody>().useGravity = true;
            card.GetComponent<CardMotion>().selected = false;
            //card.GetComponent<BoxCollider>().enabled = true;
            StartCoroutine(CardDisable(card));
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator CardDisable(GameObject card)
    {
        yield return new WaitForSeconds(1f);
        card.SetActive(false);
    }
}
