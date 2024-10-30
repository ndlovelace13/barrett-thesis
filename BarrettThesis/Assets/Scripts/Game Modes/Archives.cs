using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archives : MonoBehaviour
{
    GameObject player;
    ObjectPool cardPool;
    public List<GameObject> displayedCards;

    int currentIndex;
    int indexModAmount = 1;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        cardPool = GameObject.FindWithTag("CardPool").GetComponent<ObjectPool>();
        displayedCards = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && GameController.GameControl.gameMode == GameMode.DEFAULT)
        {
            GameController.GameControl.lockPlayer = true;
            currentIndex = 0;
            GameController.GameControl.gameMode = GameMode.ARCHIVE;
            StartCoroutine(ArchiveCam());
        }
        if (GameController.GameControl.gameMode == GameMode.ARCHIVE)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameController.GameControl.lockPlayer = false;
                GameController.GameControl.gameMode = GameMode.DEFAULT;
                StartCoroutine(RemoveCard(displayedCards));
            }

            //get archive increment
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                cardIncrement(-indexModAmount);
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                cardIncrement(indexModAmount);
        }

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
        Quaternion startingRot = Camera.main.transform.rotation;
        Quaternion endingRot = new Quaternion(0f, startingRot.y, 0f, startingRot.w);
        //lerp the camera to straight on
        while (currentTime < 0.3f)
        {
            Camera.main.transform.rotation = Quaternion.Lerp(startingRot, endingRot, currentTime / 0.3f);
            yield return new WaitForEndOfFrame();
            currentTime += Time.deltaTime;
        }
        Camera.main.transform.rotation = endingRot;

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
