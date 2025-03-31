using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class Create : CoreGameMode, IInteractable
{

    [SerializeField] PlaceableHandler allPlaceable;

    //associated Card
    Flashcard associatedCard;

    //painting handling
    GameObject painting;
    
    

    //notecard handling
    GameObject cardFront;
    GameObject cardBack;

    [Header("Initial Locs")]
    [SerializeField] Transform frontInit;
    [SerializeField] Transform backInit;
    [SerializeField] Transform brushInit;
    [SerializeField] Transform paintInit;

    [Header("PreCreate Locs")]
    [SerializeField] Transform frontPrecreate;
    [SerializeField] Transform backPrecreate;

    [Header("Create Locs")]
    [SerializeField] Transform frontCreate;
    [SerializeField] Transform backCreate;
    [SerializeField] Transform brushCreate;
    [SerializeField] Transform paintCreate;

    [Header("PostCreate Locs")]
    [SerializeField] Transform paintPostcreate;
    
    
    
    
    
    

    [SerializeField] ObjectPool cardPool;

    //brushes
    
    

    [SerializeField] GameObject brushHolder;
    [SerializeField] Brush currentBrush;
    [SerializeField] GameObject createLight;

    [SerializeField] Eraser eraser;
    public bool eraserToggled = false;

    [SerializeField] ColorSelect colorSelect;

    //state handling
    bool newQueue = false;
    bool specificArt = false;
    bool firstArt = true;

    //timer stuff
    public float lerpTime = 0.5f;
    public float artTimer = 30f;

    [SerializeField] GameObject createCanvas;
    [SerializeField] TMP_Text timerDisplay;
    [SerializeField] Button nextStage;
    [SerializeField] TMP_Text instruction;

    public enum CreateState
    {
        PRECREATE,
        CREATING,
        POSTCREATE
    };

    public CreateState createState;

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
        if (Input.GetKeyDown(KeyCode.Mouse1) && GameController.GameControl.gameMode == GameMode.CREATING)
        {
            eraser.EraserToggle();
        }
    }

    public override bool Interact()
    {
        ObjRetrieve();
        if (GameController.SaveData.newQueue.Count > 0 || GameController.SaveData.artQueue.Count > 0)
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
        createCanvas.SetActive(false);
        createLight.SetActive(false);

        //reset firstArt
        firstArt = true;

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
        //update the state
        createState = CreateState.PRECREATE;
        timerDisplay.text = "A New Archive Appears!";
        instruction.text = "Analyze information, press Ready to start creating!";
        nextStage.GetComponentInChildren<TMP_Text>().text = "Ready to Paint!";
        nextStage.GetComponent<Button>().interactable = true;

        GameObject heldObj = GetHeldObject();
        if (heldObj != null && heldObj.GetComponent<Painting>())
        {
            PaintingFill(heldObj);
        }
        else if (heldObj != null && heldObj.GetComponent<CardFill>())
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

        //turn on the spotlight
        createLight.SetActive(true);
        
        //turn on the canvas
        createCanvas.SetActive(true);

        //lerp to first view
        StartCoroutine(FirstView());
    }

    private void PaintingFill(GameObject heldObj)
    {
        //set indicator
        specificArt = true;
        newQueue = false;

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

        //reset heldObj var
        heldObj.transform.parent = null;
        player.GetComponent<PlayerInteraction>().ResetHeldObj();
    }

    private void CardFill(GameObject heldObj)
    {
        //set indicator
        specificArt = true;
        newQueue = false;

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

        //reset heldObj var
        heldObj.transform.parent = null;
        player.GetComponent<PlayerInteraction>().ResetHeldObj();
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
        if (cardFront == null)
        {
            cardFront = cardPool.GetPooledObject();
        }
        cardFront.SetActive(true);

        cardFront.GetComponent<CardFill>().CardAssign(associatedCard);

        //retrieve and fill a new card back
        if (cardBack == null)
        {
            cardBack = cardPool.GetPooledObject();
        }
        cardBack.SetActive(true);

        cardBack.GetComponent<CardFill>().CardAssign(associatedCard);

        //create a new painting for creation
        if (painting == null)
            painting = allPlaceable.RetrievePainting();

        painting.GetComponent<Painting>().associatedCard = associatedCard;


        //StartCoroutine(FirstView());
    }

    //run whenever the user is presented with a new card - allow them time to review before jumping into the art creation
    IEnumerator FirstView()
    {
        TransformTransfer(cardFront.transform, frontInit);
        TransformTransfer(cardBack.transform, backInit);


        float timer = 0f;
        while (timer < lerpTime)
        {
            cardFront.transform.position = Vector3.Lerp(frontInit.position, frontPrecreate.position, timer / lerpTime);
            cardBack.transform.position = Vector3.Lerp(backInit.position, backPrecreate.position, timer / lerpTime);

            cardFront.transform.rotation = Quaternion.Lerp(frontInit.rotation, frontPrecreate.rotation, timer / lerpTime);
            cardBack.transform.rotation = Quaternion.Lerp(backInit.rotation, backPrecreate.rotation, timer / lerpTime);

            //lerp the painting back into place if not the first art created
            if (!firstArt)
            {
                painting.transform.position = Vector3.Lerp(paintPostcreate.position, paintInit.position, timer / lerpTime);
                painting.transform.rotation = Quaternion.Lerp(paintPostcreate.rotation, paintInit.rotation, timer / lerpTime);
            }

            //increment timer
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        TransformTransfer(cardFront.transform, frontPrecreate);
        TransformTransfer(cardBack.transform, backPrecreate);
        TransformTransfer(painting.transform, paintInit);
    }

//used to lerp all objs into place
    IEnumerator ObjectLerp()
    {
        //Set init Transforms
        
        TransformTransfer(painting.transform, paintInit);
        TransformTransfer(brushHolder.transform, brushInit);
        
        //set up the timer
        float timer = 0f;

        //activate painting mode here
        painting.GetComponent<Painting>().ColliderEnable();
        painting.GetComponent<Paint>().StartPainting(associatedCard);

        //start timer
        while (timer < lerpTime)
        {
            cardFront.transform.position = Vector3.Lerp(frontPrecreate.position, frontCreate.position, timer / lerpTime);
            cardBack.transform.position = Vector3.Lerp(backPrecreate.position, backCreate.position, timer / lerpTime);
            painting.transform.position = Vector3.Lerp(paintInit.position, paintCreate.position, timer / lerpTime);
            brushHolder.transform.position = Vector3.Lerp(brushInit.position, brushCreate.position, timer / lerpTime);

            cardFront.transform.rotation = Quaternion.Lerp(frontPrecreate.rotation, frontCreate.rotation, timer / lerpTime);
            cardBack.transform.rotation = Quaternion.Lerp(backPrecreate.rotation, backCreate.rotation, timer / lerpTime);
            painting.transform.rotation = Quaternion.Lerp(paintInit.rotation, paintCreate.rotation, timer / lerpTime);
            brushHolder.transform.rotation = Quaternion.Lerp(brushInit.rotation, brushCreate.rotation, timer / lerpTime);

            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        //finalize lerp
        TransformTransfer(cardFront.transform, frontCreate);
        TransformTransfer(cardBack.transform, backCreate);
        TransformTransfer(painting.transform, paintCreate);
        TransformTransfer(brushHolder.transform, brushCreate);

        

        //select the current brush
        currentBrush.Select();

        //reset the timer
        StartCoroutine(StartTimer());
    }

    IEnumerator StartTimer()
    {
        //update the state
        createState = CreateState.CREATING;
        instruction.text = "Create a work for this archive within the alotted time!";
        nextStage.GetComponentInChildren<TMP_Text>().text = "I'm Done!";

        if (newQueue)
            artTimer = 60f;
        else
            artTimer = 30f;

        float currentTime = 0f;

        int min = Mathf.FloorToInt((artTimer - currentTime) / 60);
        int sec = Mathf.FloorToInt((artTimer - currentTime) % 60);

        while ( currentTime < artTimer)
        {
            timerDisplay.text = string.Format("{0:00}:{1:00}", min, sec);

            min = Mathf.FloorToInt((artTimer - currentTime) / 60);
            sec = Mathf.FloorToInt((artTimer - currentTime) % 60);

            currentTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();

            //break out if it has been cancelled elsewhere
            if (createState != CreateState.CREATING)
            {
                Debug.Log("Breaking out of timer loop");
                yield break;
            }
        }

        StopPainting();
    }

    IEnumerator PostCreationLerp()
    {
        Debug.Log("Reached Post Creation Lerp");

        //update the state
        createState = CreateState.POSTCREATE;
        timerDisplay.text = "Work Completed";
        firstArt = false;

        //handle cases for newQueue, artQueue, and single work
        if (newQueue && GameController.SaveData.newQueue.Count > 0)
        {
            instruction.text = "Press New Work when you're ready to create your next masterpiece!";
            nextStage.GetComponentInChildren<TMP_Text>().text = "Next Work (" + GameController.SaveData.newQueue.Count + " Remaining)";
        }
        else if (newQueue)
        {
            if (GameController.SaveData.artQueue.Count > 0)
            {
                instruction.text = "All new works created! Press Esc to return to museum or Next Work to begin remastering older pieces";
                nextStage.GetComponentInChildren<TMP_Text>().text = "Next Remaster (" + GameController.SaveData.artQueue.Count + " Remaining)";
            }
            else
            {
                instruction.text = "All new works created! Press Esc to return to museum";
                nextStage.GetComponent<Button>().interactable = false;
            }

        }
        else
        {
            if (specificArt)
            {
                timerDisplay.text = "Rework Completed";
                instruction.text = "Press Esc to return to museum or Q to take this piece for hanging";
                nextStage.GetComponent<Button>().interactable = false;
            }
            else if (GameController.SaveData.artQueue.Count > 0)
            {
                timerDisplay.text = "Remaster Completed";
                instruction.text = "Press Esc to return to museum or Next Work to begin remastering older pieces!";
                nextStage.GetComponentInChildren<TMP_Text>().text = "Next Remaster(" + GameController.SaveData.artQueue.Count + " Remaining)";
            }
            else
            {
                timerDisplay.text = "Remaster Completed";
                instruction.text = "All remasters completed! Press Esc to return to museum";
                nextStage.GetComponent<Button>().interactable = false;
            }

        }

        //do the roar
        //set up the timer
        float timer = 0f;

        //start timer
        while (timer < lerpTime)
        {
            cardFront.transform.position = Vector3.Lerp(frontCreate.position, frontInit.position, timer / lerpTime);
            cardBack.transform.position = Vector3.Lerp(backCreate.position, backInit.position, timer / lerpTime);
            painting.transform.position = Vector3.Lerp(paintCreate.position, paintPostcreate.position, timer / lerpTime);
            brushHolder.transform.position = Vector3.Lerp(brushCreate.position, brushInit.position, timer / lerpTime);

            cardFront.transform.rotation = Quaternion.Lerp(frontCreate.rotation, frontInit.rotation, timer / lerpTime);
            cardBack.transform.rotation = Quaternion.Lerp(backCreate.rotation, backInit.rotation, timer / lerpTime);
            painting.transform.rotation = Quaternion.Lerp(paintCreate.rotation, paintPostcreate.rotation, timer / lerpTime);
            brushHolder.transform.rotation = Quaternion.Lerp(brushCreate.rotation, brushInit.rotation, timer / lerpTime);

            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }



        yield return null;
    }

    //call this when time runs out or the player is done with painting early
    public void StopPainting()
    {

        

        //enable an option to continue to the next piece if part of queue
        if (newQueue)
        {
            GameController.SaveData.newQueue.Remove(associatedCard);
            GameController.SaveData.cardQueue.Add(associatedCard);
            Debug.Log("New Queue Count: " + GameController.SaveData.newQueue.Count);
        }
            
        else
        {
            if (GameController.SaveData.artQueue.Contains(associatedCard))
                GameController.SaveData.artQueue.Remove(associatedCard);
        }

        //display finalized art
        StartCoroutine(PostCreationLerp());

        painting.GetComponent<Paint>().StopPainting();


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

    public void AdvanceState()
    {
        switch (createState)
        {
            case CreateState.PRECREATE:
                StartCoroutine(ObjectLerp());
                break;
            case CreateState.CREATING:
                StopPainting();
                break;
            case CreateState.POSTCREATE:
                SetupObjects();
                break;
            default: Debug.Log("Case not handled, you're cooked buster");
                break;

        }

    }
}
