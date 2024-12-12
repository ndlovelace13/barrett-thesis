using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] Transform backStart;
    [SerializeField] Transform backEnd;

    [SerializeField] ObjectPool cardPool;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        gameMode = GameMode.CREATING;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override bool Interact()
    {
        GameObject obj = GetHeldObject();
        if (obj != null)
        {
            if (obj.GetComponent<CardFill>() != null || obj.GetComponent<Painting>() != null)
            {
                return base.Interact();
            }
        }
        return false;
    }

    //TODO - rework so that the player could retrieve the painting or card if they wanted to?
    public override void CancelInteract()
    {
        painting.GetComponent<Paint>().StopPainting();
        Destroy(painting);
        cardFront.SetActive(false);
        cardBack.SetActive(false);
        base.CancelInteract();
        
    }

    public override string GetPrompt()
    {
        GameObject obj = GetHeldObject();
        if (obj != null)
        {
            if (obj.GetComponent<CardFill>() != null || obj.GetComponent<Painting>() != null)
                return "Press E to Restore a Painting";
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
        else if (heldObj.GetComponent<CardFill>())
        {
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
        else
        {
            Debug.Log("You're boned buddy");
        }
        player.GetComponent<PlayerInteraction>().ResetHeldObj();

        StartCoroutine(ObjectLerp());
    }

    //used to lerp all objs into place
    IEnumerator ObjectLerp()
    {
        //Set init Transforms
        TransformTransfer(cardFront.transform, frontStart);
        TransformTransfer(cardBack.transform, backStart);
        TransformTransfer(painting.transform, paintStart);
        
        //set up the timer
        float timer = 0f;
        float lerpTime = 0.5f;

        //start timer
        while (timer < lerpTime)
        {
            cardFront.transform.position = Vector3.Lerp(frontStart.position, frontEnd.position, timer / lerpTime);
            cardBack.transform.position = Vector3.Lerp(backStart.position, backEnd.position, timer / lerpTime);
            painting.transform.position = Vector3.Lerp(paintStart.position, paintEnd.position, timer / lerpTime);

            cardFront.transform.rotation = Quaternion.Lerp(frontStart.rotation, frontEnd.rotation, timer / lerpTime);
            cardBack.transform.rotation = Quaternion.Lerp(backStart.rotation, backEnd.rotation, timer / lerpTime);
            painting.transform.rotation = Quaternion.Lerp(paintStart.rotation, paintEnd.rotation, timer / lerpTime);

            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        //finalize lerp
        TransformTransfer(cardFront.transform, frontEnd);
        TransformTransfer(cardBack.transform, backEnd);
        TransformTransfer(painting.transform, paintEnd);
        //activate painting mode here
        painting.GetComponent<Paint>().StartPainting(associatedCard);
    }

    //Debugging
    private void SpawnNewObj()
    {
        CancelInteract();
        GameObject newObj = allPlaceable.RandomPlaceable();
        newObj.GetComponent<IInteractable>().Interact();
        player.GetComponent<PlayerInteraction>().RearrangeObj(newObj);
        player.GetComponent<PlayerInteraction>().isInteracting = false;
    }

    public void TransformTransfer(Transform ogTrans, Transform newTrans)
    {
        ogTrans.position = newTrans.position;
        ogTrans.rotation = newTrans.rotation;
    }
}
