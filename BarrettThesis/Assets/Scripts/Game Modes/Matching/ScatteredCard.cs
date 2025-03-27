using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class ScatteredCard : Rearrangeable, IInteractable
{
    public Flashcard cardData;

    [SerializeField] MeshRenderer artwork;
    [SerializeField] MeshRenderer baseCard;
    Material paintMat;
    Texture defaultPaint;
    [SerializeField] TMP_Text promptText;
    [SerializeField] Transform cardTransform;
    [SerializeField] Animator animControl;

    public float lerpTime = 1f;

    //core game mode objects for later calls
    protected Archives archives;
    protected Matching matchControl;
    protected MatchScanner matchScanner;
    bool inspecting = false;

    // Start is called before the first frame update
    protected override void Awake()
    { 
        paintMat = artwork.material;
        defaultPaint = paintMat.mainTexture;
        Debug.Log(paintMat.ToString());

        
        //Debug.Log("core objects found");

        base.Awake();
    }

    private void GetCoreObjs()
    {
        //store the core game mode objects for later calls
        if (archives == null)
            archives = GameObject.FindWithTag("Archives").GetComponent<Archives>();
        if (matchControl == null)
            matchControl = GameObject.FindWithTag("Matching").GetComponent<Matching>();
        if (matchScanner == null)
            matchScanner = GameObject.FindFirstObjectByType<MatchScanner>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override Vector3 PlaceOffset(GameObject wall)
    {
        Vector3 baseVector = Vector3.zero;
        if (wall.tag == "Wall")
            baseVector = wall.transform.right;
        else if (wall.tag == "Ceiling")
            baseVector = -wall.transform.up;
        else if (wall.tag == "Floor")
            baseVector = wall.transform.up;

        float offset = cardTransform.lossyScale.x / 2f;
        return baseVector * offset;
    }

    public override void Place(RaycastHit hit)
    {
        inPlace = true;
        GameObject currentWall = hit.collider.gameObject;

        

        if (currentWall.tag == "Wall")
            transform.rotation = Quaternion.Euler(currentWall.transform.rotation.eulerAngles);
        else if (currentWall.tag == "Ceiling")
            transform.rotation = Quaternion.Euler(0f, 0f, -90f);
        else if (currentWall.tag == "Floor")
            transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        else
            Debug.Log("Shit is fucked" + currentWall.tag);

        //Debug.Log(transform.rotation.eulerAngles);
        transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z) + PlaceOffset(currentWall);

        RotateOffset();
        Debug.Log("ScatteredCard Placed at " + transform.position);
    }

    public void RotateOffset()
    {
        float xRot = Random.Range(-45f, 45f);
        Vector3 tempRot = transform.rotation.eulerAngles + new Vector3(xRot, 0f, 0f);
        transform.rotation = Quaternion.Euler(tempRot);
    }

    public void ArchiveView(Flashcard newCard, Vector3 finalPos)
    {
        if (newCard.discovered)
            FillCard(newCard);
        else
            UndiscoveredFill(newCard);
        ApplyMastery();

        GetCoreObjs();

        //lerp from the archive
        FromArchive(finalPos);
    }

    public void UndiscoveredFill(Flashcard newCard)
    {
        Debug.Log("Card not discovered!");
        cardData = newCard;
        promptText.text = "???";
        paintMat.mainTexture = defaultPaint;
    }

    public void FillCard(Flashcard newCard)
    {
        cardData = newCard;
        if (cardData.useCustom)
        {
            Debug.Log(paintMat.mainTexture);
            Debug.Log(cardData.customArt);
            paintMat.mainTexture = SaveHandler.SaveSystem.GetPainting(cardData.customArt);
        }
            
        NoteType noteInfo = GameController.SaveData.currentDeck.dictRetrieve(cardData.noteId);
        promptText.text = cardData.fields[noteInfo.matchPromptField];

        GetCoreObjs();
    }

    public Flashcard ReportCard()
    {
        return cardData;
    }

    public void ApplyMastery()
    {
        Debug.Log("Mastery Material Applied");
        if (cardData.discovered)
            baseCard.material = DeckManager.DeckManage.masteryMaterials[cardData.masteryLevel];
        else
            baseCard.material = DeckManager.DeckManage.mysteryMat;
    }

    public void CorrectBehavior(Transform finalLoc)
    {
        Debug.Log("Correct Behavior Called");

        StartCoroutine(CorrectLerp(finalLoc));
    }

    IEnumerator CorrectLerp(Transform finalLoc)
    {
        /*store starting pos
        Vector3 startPosition = transform.localPosition;

        //execute lerp
        float currentTime = 0f;
        while (currentTime < lerpTime)
        {
            transform.localPosition = Vector3.Lerp(startPosition, finalLoc.position, currentTime / lerpTime);
            //Debug.Log(transform.position);
            currentTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        transform.localPosition = finalLoc.position;*/

        //execute anim
        animControl.SetTrigger("correct");
        //make an animation trigger in the animation to call the second half of this function instead of relying on animation time tracking
        /*while (animControl.playbackTime < 1f)
        {
            yield return new WaitForFixedUpdate();
        }*/

        //lerp over shoulder and kill
        

        yield return null;
    }

    public void IncorrectBehavior()
    {
        Debug.Log("Incorrect Behavior Called");
        animControl.SetTrigger("incorrect");
        //StartCoroutine(Incorrect)
    }

    public void SpinComplete()
    {
        Debug.Log("SpinComplete");
        StartCoroutine(CardRemoval());   
    }

    public void ShakeComplete()
    {
        if (transform.parent != null)
        {
            transform.parent.GetComponent<SprenBehavior>().SprenDeselect();
        }
        else
            Debug.Log("What the sigma");
    }

    IEnumerator CardRemoval()
    {
        yield return new WaitForSeconds(1f);
        if (transform.parent != null)
            transform.parent.gameObject.SetActive(false);
        else
            gameObject.SetActive(false);

        Debug.Log("Reached end of correct Lerp");
        yield return null;
    }

    public void OnMouseEnter()
    {
        if (!inspecting)
        {
            //enable the outline, enable a mouse over animation?
            ActivateHighlight();
        }

        //Debug.Log("Mouse over detected");
    }

    public void OnMouseExit()
    {
        //disable the outline and animation
        DeactivateHighlight();
    }

    public void OnMouseDown()
    {
        //if currentState is Matching lerp to the player for matching, fill out match scanner
        if (GameController.GameControl.gameMode == GameMode.MATCHING)
        {
            Debug.Log("Matching Click Detected");
            matchScanner.CardSelect(transform.root.gameObject, cardData);
        }
        else if (GameController.GameControl.gameMode == GameMode.ARCHIVE)
        {
            Debug.Log("Archive Click detected");
            archives.SpecificFileView(gameObject, cardData);
            inspecting = true;
        }
        else
        {
            Debug.Log("Click detected outside of two handled modes");
        }


        //if currentState is Archive, enable the specific view
        
    }

    public void StopInspect()
    {
        inspecting = false;
    }

    public void FromArchive(Vector3 finalPos)
    {
        StartCoroutine(ArchiveLerp(archives.transform.position, finalPos, false));
    }

    public void ToArchive()
    {
        StartCoroutine(ArchiveLerp(transform.position, archives.transform.position, true));
    }

    IEnumerator ArchiveLerp(Vector3 startPos, Vector3 endPos, bool disable)
    {

        float currentTime = 0f;
        while (currentTime < GameController.GameControl.lerpTime)
        {
            transform.position = Vector3.Lerp(startPos, endPos, currentTime / GameController.GameControl.lerpTime);

            currentTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        transform.position = endPos;

        if (disable)
            gameObject.SetActive(false);
    }


}
