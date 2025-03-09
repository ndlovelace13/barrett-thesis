using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archives : CoreGameMode, IInteractable
{
    ObjectPool cardPool;
    ObjectPool scatteredPool;
    ObjectPool statsPool;
    public List<GameObject> displayedCards;

    [SerializeField] Transform displayGrid;

    [Header("File View Locations")]
    [SerializeField] Transform scatteredLoc;
    [SerializeField] Transform flashcardLoc;
    [SerializeField] Transform statsLoc;

    //cam control
    //public Transform camControl;

    int currentIndex;
    int indexModAmount = 15;
    int numPerRow = 5;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        cardPool = GameObject.FindWithTag("CardPool").GetComponent<ObjectPool>();
        scatteredPool = GameObject.FindWithTag("ScatteredPool").GetComponent<ObjectPool>();
        statsPool = GameObject.FindWithTag("StatsPool").GetComponent<ObjectPool>();
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

    public override bool CancelInteract()
    {
        base.CancelInteract();
        //camControl.rotation = Camera.main.transform.rotation;
        StartCoroutine(RemoveCard(displayedCards));
        return true;
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

        //set the starting row to be centered around 0
        int currentRow = 0 + ((indexModAmount / numPerRow) / 2);
        int startingZ = 0 + (numPerRow / 2);
        int zPos = startingZ;

        for (int i = 0; i < cardNum; i++)
        {
            GameObject currentCard = scatteredPool.GetPooledObject();
            currentCard.SetActive(true);

            //set card grid a certain dist from the user
            displayGrid.position = Camera.main.transform.position + Camera.main.transform.forward * 3f;

            

            //set the card to the displayGrid
            currentCard.transform.SetParent(displayGrid, false);

            //check for a row increment otherwise increment zPos
            if (i != 0 && i % numPerRow == 0)
            {
                currentRow--;
                zPos = startingZ;
            }
            else if (i != 0)
            {
                zPos--;
            }
                

            //calculate position in the grid

            currentCard.transform.localPosition = new Vector3(0, currentRow, zPos);
            /*currentCard.GetComponent<Rigidbody>().useGravity = false;
            currentCard.GetComponent<Rigidbody>().velocity = Vector3.zero;
            currentCard.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;*/
            //currentCard.GetComponent<BoxCollider>().enabled = false;

            //in case need to set rotation
            displayGrid.transform.LookAt(Camera.main.transform);
            displayGrid.transform.Rotate(0f, 90f, 0f);
            //currentCard.transform.rotation = currentCam;

            //fill the card
            Flashcard currentFlashcard = GameController.SaveData.currentDeck.cards[cardIndex + i];
            //currentCard.GetComponent<CardFill>().CardAssign(currentFlashcard);
            currentCard.GetComponent<ScatteredCard>().ArchiveView(currentFlashcard);
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
            //card.GetComponent<Rigidbody>().useGravity = true;
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

    public void SpecificFileView(GameObject clickedFile, Flashcard currentCard)
    {
        //spawn in a flashcard and fill with the current card info
        GameObject newCard = cardPool.GetPooledObject();
        newCard.SetActive(true);
        newCard.GetComponent<CardFill>().CardAssign(currentCard);
        newCard.transform.position = clickedFile.transform.position;

        //spawn in a stats card and fill with the current card info
        GameObject newStats = statsPool.GetPooledObject();
        newStats.SetActive(true);
        newStats.transform.position = clickedFile.transform.position;
        newStats.GetComponent<StatsCard>().AssignCard(currentCard);

        //call the lerp
        StartCoroutine(EnterFileView(clickedFile, newCard, newStats));

        
    }

    IEnumerator EnterFileView(GameObject scatteredCard, GameObject flashcard, GameObject statsCard)
    {
        //set starting locations
        Vector3 scatteredStart = scatteredCard.transform.position;
        Vector3 flashcardStart = flashcard.transform.position;
        Vector3 statsStart = statsCard.transform.position;


        //begin lerp
        float currentTime = 0f;
        while (currentTime < GameController.GameControl.lerpTime)
        {
            //update all locations
            scatteredCard.transform.position = Vector3.Lerp(scatteredStart, scatteredLoc.position, currentTime / GameController.GameControl.lerpTime);
            flashcard.transform.position = Vector3.Lerp(flashcardStart, flashcardLoc.position, currentTime / GameController.GameControl.lerpTime);
            statsCard.transform.position = Vector3.Lerp(statsStart, statsLoc.position, currentTime / GameController.GameControl.lerpTime);

            //increment the timer
            currentTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        //set to final locations
        scatteredCard.transform.position = scatteredLoc.position;
        flashcard.transform.position = flashcardLoc.position;
        statsCard.transform.position = statsLoc.position;

    }
}
