using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archives : Interactable
{
    GameObject player;
    PlayerCam cam;
    ObjectPool cardPool;
    public List<GameObject> displayedCards;

    //cam control
    public Transform camControl;

    int currentIndex;
    int indexModAmount = 1;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        cam = Camera.main.GetComponent<PlayerCam>();
        cardPool = GameObject.FindWithTag("CardPool").GetComponent<ObjectPool>();
        displayedCards = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.GameControl.gameMode == GameMode.ARCHIVE)
        {
            //get archive increment
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                cardIncrement(-indexModAmount);
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                cardIncrement(indexModAmount);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            CardRetrieve();
        }
    }

    public override void Interact()
    {
        base.Interact();
        GameController.GameControl.lockPlayer = true;
        currentIndex = 0;
        GameController.GameControl.gameMode = GameMode.ARCHIVE;
        StartCoroutine(ArchiveCam());
    }

    public override void CancelInteract()
    {
        base.CancelInteract();
        GameController.GameControl.lockPlayer = false;
        camControl.rotation = Camera.main.transform.rotation;
        GameController.GameControl.gameMode = GameMode.DEFAULT;
        StartCoroutine(RemoveCard(displayedCards));
    }

    public void CardRetrieve()
    {
        GameObject currentCard = displayedCards[0];
        displayedCards.Remove(currentCard);
        currentCard.transform.SetParent(GameObject.FindWithTag("ObjectSlot").transform, false);
        currentCard.transform.localPosition = Vector3.zero;
        currentCard.transform.localRotation = Quaternion.Euler(Vector3.zero);
        //transform.localScale = transform.localScale / 2f; 
        currentCard.GetComponent<ObjectMotion>().held = true;
        currentCard.GetComponentInChildren<Collider>().enabled = false;
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

    IEnumerator ArchiveCam()
    {
        float currentTime = 0f;
        float camStart = cam.xRotation;
        //Quaternion startingRot = Camera.main.transform.rotation;
        //Quaternion endingRot = new Quaternion(0f, startingRot.y, 0f, startingRot.w);
        //lerp the camera to straight on
        while (currentTime < 0.3f)
        {
            cam.xRotation = Mathf.Lerp(camStart, 0, currentTime / 0.3f);
            //Camera.main.transform.rotation = Quaternion.Lerp(startingRot, endingRot, currentTime / 0.3f);
            yield return new WaitForEndOfFrame();
            currentTime += Time.deltaTime;
        }
        //Camera.main.transform.rotation = endingRot;
        cam.xRotation = 0f;

        StartCoroutine(SpawnCard(currentIndex, indexModAmount));
        yield return null;
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
