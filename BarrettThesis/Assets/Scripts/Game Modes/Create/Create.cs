using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Create : CoreGameMode, IInteractable
{

    [SerializeField] PlaceableHandler allPlaceable;

    //associated Card
    Flashcard associatedCard;

    //painting handling
    GameObject painting;
    [SerializeField] Transform paintStart;
    [SerializeField] Transform paintEnd;

    //notecard handling
    GameObject cardFront;
    GameObject cardBack;

    [SerializeField] Transform frontStart;
    [SerializeField] Transform frontEnd;
    [SerializeField] Transform frontPresent;
    [SerializeField] Transform backStart;
    [SerializeField] Transform backEnd;
    [SerializeField] Transform backPresent;

    [SerializeField] ObjectPool cardPool;

    //brushes
    [SerializeField] Transform brushStart;
    [SerializeField] Transform brushEnd;

    [SerializeField] GameObject brushHolder;
    [SerializeField] Brush currentBrush;

    [SerializeField] Eraser eraser;
    public bool eraserToggled = false;

    [SerializeField] ColorSelect colorSelect;

    //state handling
    bool newQueue = false;
    bool specificArt = false;

    //timer stuff
    public float lerpTime = 0.5f;
    public float artTimer = 30f;

    [SerializeField] TMP_Text timerDisplay;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        gameMode = GameMode.CREATING;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && GameController.GameControl.gameMode == GameMode.CREATING)
        {
            PaintingRetrieve();
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            eraser.EraserToggle();
        }
    }

    public override bool Interact()
    {
        ObjRetrieve();
        if (GameController.SaveData.newQueue.Count > 0)
        {
            return base.Interact();
        }
        else
        {
            GameObject obj = GetHeldObject();
            if (obj != null)
            {
                if (obj.GetComponent<CardFill>() != null || obj.GetComponent<Painting>() != null)
                {
                    return base.Interact();
                }
            }
        }
        return false;
    }

    private void ObjRetrieve()
    {
        if (allPlaceable == null)
            allPlaceable = GameObject.FindObjectOfType<PlaceableHandler>();
        if (cardPool == null)
            cardPool = GameObject.FindWithTag("CardPool").GetComponent<ObjectPool>();
    }

    private void PaintingRetrieve()
    {
        if (GameObject.FindObjectsOfType<Painting>().Length < allPlaceable.controlDict[PlaceableType.Painting].currentMax)
        {
            painting.GetComponent<Paint>().StopPainting();
            painting.GetComponent<Rearrangeable>().Interact();
            painting = null;
            CancelInteract();
        }
        else
        {
            Debug.Log("Max Paintings Reached");
        }
    }

    //TODO - rework so that the player could retrieve the painting or card if they wanted to?
    public override bool CancelInteract()
    {
        if (painting != null)
        {
            painting.GetComponent<Paint>().StopPainting();
            Destroy(painting);
        }
        cardFront.SetActive(false);
        cardBack.SetActive(false);
        brushHolder.SetActive(false);
        return base.CancelInteract();
        
    }

    public override string GetPrompt()
    {
        GameObject obj = GetHeldObject();
        if (obj != null)
        {
            if (obj.GetComponent<CardFill>() != null || obj.GetComponent<Painting>() != null)
                return "Press E to Restore a Painting";
        }
        else
        {
            if (GameController.SaveData.newQueue.Count > 0)
                return "Press E to Add New Art to Archive";
            else if (GameController.SaveData.artQueue.Count > 0)
                return "Press E to Touch Up Existing Art";
        }
        return "Must be holding a card or painting in order to restore";
    }

    protected override void PostCameraShift()
    {
        base.PostCameraShift();
        SetupObjects();
    }

    private void SetupObjects()
    {
        GameObject heldObj = GetHeldObject();
        if (heldObj.GetComponent<Painting>())
        {
            PaintingFill(heldObj);
        }
        else if (heldObj.GetComponent<CardFill>())
        {
            CardFill(heldObj);
        }
        else
        {
            //cases for newQueue art needing to be made and artQueue art needing to be added to
            if (GameController.SaveData.newQueue.Count > 0)
                QueuePrepare(true);
            else if (GameController.SaveData.artQueue.Count > 0)
                QueuePrepare(false);
        }

        //turn on brush objects
        brushHolder.SetActive(true);

        heldObj.transform.parent = null;
        player.GetComponent<PlayerInteraction>().ResetHeldObj();

        StartCoroutine(ObjectLerp());
    }

    private void PaintingFill(GameObject heldObj)
    {
        //set indicator
        specificArt = true;

        //associate the painting 
        painting = heldObj;
        associatedCard = painting.GetComponent<Painting>().associatedCard;

        //retrieve and fill new card front
        cardFront = cardPool.GetPooledObject();
        cardFront.SetActive(true);
        cardFront.GetComponent<CardFill>().CardAssign(associatedCard);

        //retrieve and fill new card back
        cardBack = cardPool.GetPooledObject();
        cardBack.SetActive(true);
        cardBack.GetComponent<CardFill>().CardAssign(associatedCard);
    }

    private void CardFill(GameObject heldObj)
    {
        //set indicator
        specificArt = true;

        //associate the card front
        cardFront = heldObj;
        associatedCard = cardFront.GetComponent<CardFill>().GetFlashcard();

        //create a new painting for creation
        painting = allPlaceable.RetrievePainting();
        painting.GetComponent<Painting>().associatedCard = associatedCard;
        painting.GetComponent<Painting>().AssignImage(false);

        //retrieve and fill a new card back
        cardBack = cardPool.GetPooledObject();
        cardBack.SetActive(true);
        cardBack.GetComponent<CardFill>().CardAssign(associatedCard);
    }

    private void QueuePrepare(bool newCards)
    {
        //set indicator bools
        specificArt = false;
        newQueue = newCards;

        //pull the first card in the queue
        if (newQueue)
        {
            associatedCard = GameController.SaveData.newQueue.First();
        }
        else
        {
            associatedCard = GameController.SaveData.artQueue.First();
        }

        //retrieve and fill new card front
        cardFront = cardPool.GetPooledObject();
        cardFront.SetActive(true);
        cardFront.GetComponent<CardFill>().CardAssign(associatedCard);

        //retrieve and fill a new card back
        cardBack = cardPool.GetPooledObject();
        cardBack.SetActive(true);
        cardBack.GetComponent<CardFill>().CardAssign(associatedCard);

        //create a new painting for creation
        painting = allPlaceable.RetrievePainting();
        painting.GetComponent<Painting>().associatedCard = associatedCard;


        StartCoroutine(FirstView());
    }

    //run whenever the user is presented with a new card - allow them time to review before jumping into the art creation
    IEnumerator FirstView()
    {
        TransformTransfer(cardFront.transform, frontStart);
        TransformTransfer(cardBack.transform, backStart);


        float timer = 0f;
        while (timer < lerpTime)
        {
            cardFront.transform.position = Vector3.Lerp(frontStart.position, frontPresent.position, timer / lerpTime);
            cardBack.transform.position = Vector3.Lerp(backStart.position, backPresent.position, timer / lerpTime);

            cardFront.transform.rotation = Quaternion.Lerp(frontStart.rotation, frontPresent.rotation, timer / lerpTime);
            cardBack.transform.rotation = Quaternion.Lerp(backStart.rotation, backPresent.rotation, timer / lerpTime);

            //increment timer
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

//used to lerp all objs into place
    IEnumerator ObjectLerp()
    {
        //Set init Transforms
        TransformTransfer(cardFront.transform, frontStart);
        TransformTransfer(cardBack.transform, backStart);
        TransformTransfer(painting.transform, paintStart);
        TransformTransfer(brushHolder.transform, brushStart);
        
        //set up the timer
        float timer = 0f;

        //start timer
        while (timer < lerpTime)
        {
            cardFront.transform.position = Vector3.Lerp(frontStart.position, frontEnd.position, timer / lerpTime);
            cardBack.transform.position = Vector3.Lerp(backStart.position, backEnd.position, timer / lerpTime);
            painting.transform.position = Vector3.Lerp(paintStart.position, paintEnd.position, timer / lerpTime);
            brushHolder.transform.position = Vector3.Lerp(brushStart.position, brushEnd.position, timer / lerpTime);

            cardFront.transform.rotation = Quaternion.Lerp(frontStart.rotation, frontEnd.rotation, timer / lerpTime);
            cardBack.transform.rotation = Quaternion.Lerp(backStart.rotation, backEnd.rotation, timer / lerpTime);
            painting.transform.rotation = Quaternion.Lerp(paintStart.rotation, paintEnd.rotation, timer / lerpTime);
            brushHolder.transform.rotation = Quaternion.Lerp(brushStart.rotation, brushEnd.rotation, timer / lerpTime);

            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        //finalize lerp
        TransformTransfer(cardFront.transform, frontEnd);
        TransformTransfer(cardBack.transform, backEnd);
        TransformTransfer(painting.transform, paintEnd);
        TransformTransfer(brushHolder.transform, brushEnd);

        //activate painting mode here
        painting.GetComponent<Painting>().ColliderEnable();
        painting.GetComponent<Paint>().StartPainting(associatedCard);

        //select the current brush
        currentBrush.Select();

        //reset the timer
        StartTimer();
    }

    IEnumerator StartTimer()
    {
        if (newQueue)
            artTimer = 60f;
        else
            artTimer = 30f;

        float currentTime = 0f;

        int min = Mathf.FloorToInt(artTimer / 60);
        int sec = Mathf.FloorToInt(artTimer % 60);

        while ( currentTime < artTimer)
        {
            timerDisplay.text = string.Format("{0:00}:{1:00}", min, sec);

            min = Mathf.FloorToInt(artTimer / 60);
            sec = Mathf.FloorToInt(artTimer % 60);

            currentTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        StopPainting();
    }

    //call this when time runs out or the player is done with painting early
    public void StopPainting()
    {
        painting.GetComponent<Paint>().StopPainting();

        //display finalized art

        //enable an option to continue to the next piece if part of queue

        //if a single work, either submit to archives or claim the painting to hang
    }

    public void BrushSelect(Brush newBrush)
    {
        currentBrush.Deselect();
        currentBrush = newBrush;
        painting.GetComponent<Paint>().brushSize = currentBrush.GetSize();
    }

    public void ColorSelect()
    {
        Color newColor;
        if (eraserToggled)
            newColor = Color.white;
        else
            newColor = colorSelect.selectedCol;

        //assign the color to the painting
        painting.GetComponent<Paint>().brushColor = newColor;
    }

    //Debugging
    /*private void SpawnNewObj()
    {
        CancelInteract();
        GameObject newObj = allPlaceable.RandomPlaceable();
        newObj.GetComponent<IInteractable>().Interact();
        player.GetComponent<PlayerInteraction>().RearrangeObj(newObj);
        player.GetComponent<PlayerInteraction>().isInteracting = false;
    }*/

    public void TransformTransfer(Transform ogTrans, Transform newTrans)
    {
        ogTrans.position = newTrans.position;
        ogTrans.rotation = newTrans.rotation;
    }
}
