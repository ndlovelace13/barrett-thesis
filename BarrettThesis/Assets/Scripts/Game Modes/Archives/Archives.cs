using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Archives : CoreGameMode, IInteractable
{
    ObjectPool cardPool;
    ObjectPool scatteredPool;
    ObjectPool statsPool;
    public List<GameObject> displayedCards;

    [SerializeField] Transform displayGrid;

    [Header("UI Elements")]
    [SerializeField] Canvas archiveCanvas;
    [SerializeField] TMP_Text completionText;
    [SerializeField] TMP_Text displayedCardIndices;

    [Header("File View Locations")]
    [SerializeField] Transform scatteredLoc;
    [SerializeField] Transform flashcardLoc;
    [SerializeField] Transform statsLoc;

    //cam control
    //public Transform camControl;

    int currentIndex;
    int indexModAmount = 15;
    int numPerRow = 5;

    //single vs. group view vars
    bool singleView = false;
    GameObject inspectedCard;
    int inspectedIndex;
    Vector3 gridViewPos;

    //singleview objs
    GameObject singleFlash;
    GameObject singleStats;
    bool singleLocsSet = false;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        cardPool = GameObject.FindWithTag("CardPool").GetComponent<ObjectPool>();
        scatteredPool = GameObject.FindWithTag("ScatteredPool").GetComponent<ObjectPool>();
        statsPool = GameObject.FindWithTag("StatsPool").GetComponent<ObjectPool>();
        displayedCards = new List<GameObject>();
        gameMode = GameMode.ARCHIVE;
        archiveCanvas.enabled = false;
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
        //archiveCanvas.enabled = true;
        return base.Interact();
        //StartCoroutine(ArchiveCam());
    }

    public override bool CancelInteract()
    {
        if (!singleView)
        {
            base.CancelInteract();
            //camControl.rotation = Camera.main.transform.rotation;
            StartCoroutine(RemoveCard(displayedCards));
            archiveCanvas.enabled = false;
            return true;
        }
        else
        {
            StartCoroutine(ExitFileView());
            return false;
        }
        
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
        else
        {
            Debug.Log("Index outside of range");
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

        //set card grid a certain dist from the user
        displayGrid.position = Camera.main.transform.position + Camera.main.transform.forward * 3f;

        //in case need to set rotation
        displayGrid.transform.LookAt(Camera.main.transform);
        displayGrid.transform.Rotate(0f, 90f, 0f);

        EnableCanvas();

        for (int i = 0; i < cardNum; i++)
        {
            //check for a row increment otherwise increment zPos
            //MUST be done regardless of whether a card is created or not
            if (i != 0 && i % numPerRow == 0)
            {
                currentRow--;
                zPos = startingZ;
            }
            else if (i != 0)
            {
                zPos--;
            }

            if (cardIndex + i == inspectedIndex && singleView)
            {
                //just add the inspectedCard to the newCard list if currently in singleView and an inspectedIndex exists
                newCards.Add(inspectedCard);
                inspectedCard.GetComponent<ScatteredCard>().StopInspect();
            }
            else
            {
                GameObject currentCard = scatteredPool.GetPooledObject();
                currentCard.SetActive(true);

                //set the card to the displayGrid
                currentCard.transform.SetParent(displayGrid, false);

                //currentCard.transform.localPosition = new Vector3(0, currentRow, zPos);
                //translate the given localpos into absolute location
                Vector3 endPos = displayGrid.transform.TransformPoint(new Vector3(0, currentRow, zPos));

                //fill the card
                Flashcard currentFlashcard = GameController.SaveData.currentDeck.cards[cardIndex + i];
                //currentCard.GetComponent<CardFill>().CardAssign(currentFlashcard);
                currentCard.GetComponent<ScatteredCard>().ArchiveView(currentFlashcard, endPos);
                currentCard.GetComponent<CardMotion>().selected = true;

                //add the new card to the newCards list
                newCards.Add(currentCard);
            }
            yield return new WaitForFixedUpdate();
        }

        singleView = false;
        displayedCards = newCards;
        Debug.Log("Displayed Cards: " +  displayedCards.Count);
        yield return null;
    }

    private void EnableCanvas()
    {
        archiveCanvas.enabled = true;

        //update the ui elements
        completionText.text = ((float)GameController.SaveData.unlockedCardCount / (float)GameController.SaveData.currentDeck.cards.Count * 100).ToString("F2") + "% Cards Discovered";
        displayedCardIndices.text = "Cards " + (currentIndex + 1) + "-" + (currentIndex + indexModAmount);
    }

    IEnumerator RemoveCard(List<GameObject> currentCards)
    {
        foreach (GameObject card in currentCards)
        {
            if (card == inspectedCard && singleView)
                continue;
            Debug.Log("Removing Card");
            //card.GetComponent<Rigidbody>().useGravity = true;
            card.GetComponent<CardMotion>().selected = false;
            //card.GetComponent<BoxCollider>().enabled = true;
            //StartCoroutine(CardDisable(card));
            card.GetComponent<ScatteredCard>().ToArchive();

            yield return new WaitForFixedUpdate();
        }
    }

    //NOT USED ANYMORE - called in scatteredCard directly
    IEnumerator CardDisable(GameObject card)
    {
        //lerp here instead
        yield return new WaitForSeconds(1f);
        card.SetActive(false);
    }

    public void SpecificFileView(GameObject clickedFile, Flashcard currentCard)
    {
        if (!singleView)
        {
            //display different ui here? - disabled for now
            archiveCanvas.enabled = false;

            //set single view locations if not already done so
            if (!singleLocsSet)
            {
                SetFinalLocations();
            }


            //store single view varibales
            singleView = true;
            inspectedCard = clickedFile;
            inspectedIndex = currentCard.cardId;
            gridViewPos = clickedFile.transform.position;

            //remove all currently visible cards
            StartCoroutine(RemoveCard(displayedCards));

            //spawn in a flashcard and fill with the current card info
            singleFlash = cardPool.GetPooledObject();
            singleFlash.SetActive(true);
            singleFlash.GetComponent<CardFill>().CardAssign(currentCard);
            singleFlash.transform.position = clickedFile.transform.position;
            singleFlash.transform.rotation = clickedFile.transform.rotation;
            

            //spawn in a stats card and fill with the current card info
            singleStats = statsPool.GetPooledObject();
            singleStats.SetActive(true);
            singleStats.transform.position = clickedFile.transform.position;
            singleStats.transform.rotation = clickedFile.transform.rotation;
            singleStats.GetComponent<StatsCard>().AssignCard(currentCard);

            //call the lerp
            StartCoroutine(EnterFileView());
        }
        else
        {
            Debug.Log("Already inspecting a card");
        }

        
    }

    private void SetFinalLocations()
    {
        //disable the trigger
        singleLocsSet = true;

        float zDist = 1f;
        scatteredLoc.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 0.25f, Screen.height * 0.5f, zDist));
        flashcardLoc.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 0.75f, 2 * Screen.height / 3f, zDist));
        statsLoc.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 0.75f, Screen.height / 3f, zDist));
        Debug.Log("Final Locations Set");
    }

    IEnumerator EnterFileView()
    {
        //set starting locations
        Vector3 scatteredStart = inspectedCard.transform.position;
        Vector3 flashcardStart = singleFlash.transform.position;
        Vector3 statsStart = singleStats.transform.position;

        //Quaternion scatteredStartAngle = inspectedCard.transform.rotation;
        Quaternion flashStartAngle = singleFlash.transform.rotation;
        Quaternion statsStartAngle = singleStats.transform.rotation;

        //retrieve end rotations
        //Quaternion scatteredAngle = LookAtAngle(scatteredStart);
        Quaternion flashAngle = LookAtAngle(flashcardLoc.position);
        Quaternion statsAngle = LookAtAngle(statsLoc.position);


        //begin lerp
        float currentTime = 0f;
        while (currentTime < GameController.GameControl.lerpTime)
        {
            //update all locations
            inspectedCard.transform.position = Vector3.Lerp(scatteredStart, scatteredLoc.position, currentTime / GameController.GameControl.lerpTime);
            singleFlash.transform.position = Vector3.Lerp(flashcardStart, flashcardLoc.position, currentTime / GameController.GameControl.lerpTime);
            singleStats.transform.position = Vector3.Lerp(statsStart, statsLoc.position, currentTime / GameController.GameControl.lerpTime);

            //update all the rotations
            //inspectedCard.transform.rotation = Quaternion.Lerp(scatteredStartAngle, scatteredAngle, currentTime / GameController.GameControl.lerpTime);
            singleFlash.transform.rotation = Quaternion.Lerp(flashStartAngle, flashAngle, currentTime / GameController.GameControl.lerpTime);
            singleStats.transform.rotation = Quaternion.Lerp(statsStartAngle, statsAngle, currentTime / GameController.GameControl.lerpTime);

            //increment the timer
            currentTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        //set to final locations
        inspectedCard.transform.position = scatteredLoc.position;
        singleFlash.transform.position = flashcardLoc.position;
        singleStats.transform.position = statsLoc.position;

        //set final rotations
        //inspectedCard.transform.LookAt(Camera.main.transform.position);
        singleFlash.transform.LookAt(Camera.main.transform.position);
        singleStats.transform.LookAt(Camera.main.transform.position);
        
    }

    //called when esc is pressed to return to gridview
    IEnumerator ExitFileView()
    {
        //set starting locations
        Vector3 scatteredStart = inspectedCard.transform.position;
        Vector3 flashcardStart = singleFlash.transform.position;
        Vector3 statsStart = singleStats.transform.position;

        //enable card fill for the other cards here
        StartCoroutine(SpawnCard(currentIndex, indexModAmount));

        //begin lerp
        float currentTime = 0f;
        while (currentTime < GameController.GameControl.lerpTime)
        {
            //update all locations
            inspectedCard.transform.position = Vector3.Lerp(scatteredStart, gridViewPos, currentTime / GameController.GameControl.lerpTime);
            singleFlash.transform.position = Vector3.Lerp(flashcardStart, gridViewPos, currentTime / GameController.GameControl.lerpTime);
            singleStats.transform.position = Vector3.Lerp(statsStart, gridViewPos, currentTime / GameController.GameControl.lerpTime);

            //increment the timer
            currentTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        //set to final locations
        inspectedCard.transform.position = gridViewPos;
        singleFlash.transform.position = gridViewPos;
        singleStats.transform.position = gridViewPos;

        //disable the non-file objs
        singleFlash.SetActive(false);
        singleStats.SetActive(false);

        


        yield return null;
    }

    private Quaternion LookAtAngle(Vector3 objPos)
    {
        Vector3 dir = objPos - Camera.main.transform.position;
        return Quaternion.LookRotation(-dir);
    }
}
