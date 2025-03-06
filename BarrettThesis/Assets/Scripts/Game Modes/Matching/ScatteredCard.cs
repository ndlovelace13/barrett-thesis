using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Localization.Plugins.XLIFF.Common;

public class ScatteredCard : Rearrangeable, IInteractable
{
    Flashcard cardData;

    [SerializeField] MeshRenderer artwork;
    [SerializeField] MeshRenderer baseCard;
    Material paintMat;
    [SerializeField] TMP_Text promptText;
    [SerializeField] Transform cardTransform;
    [SerializeField] Animator animControl;

    public float lerpTime = 1f;

    // Start is called before the first frame update
    protected override void Awake()
    { 
        paintMat = artwork.material;
        Debug.Log(paintMat.ToString());
        base.Awake();
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
    }

    public void RotateOffset()
    {
        float xRot = Random.Range(-45f, 45f);
        Vector3 tempRot = transform.rotation.eulerAngles + new Vector3(xRot, 0f, 0f);
        transform.rotation = Quaternion.Euler(tempRot);
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
    }

    public Flashcard ReportCard()
    {
        return cardData;
    }

    public void ApplyMastery()
    {
        Debug.Log("Mastery Material Applied");
        baseCard.material = DeckManager.DeckManage.masteryMaterials[cardData.masteryLevel];
    }

    public void CorrectBehavior(Transform finalLoc)
    {
        Debug.Log("Correct Behavior Called");

        StartCoroutine(CorrectLerp(finalLoc));
    }

    IEnumerator CorrectLerp(Transform finalLoc)
    {
        //store starting pos
        Vector3 startPosition = transform.position;

        //execute lerp
        float currentTime = 0f;
        while (currentTime < lerpTime)
        {
            transform.position = Vector3.Lerp(startPosition, finalLoc.position, currentTime / lerpTime);
            //Debug.Log(transform.position);
            currentTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        transform.position = finalLoc.position;

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

    IEnumerator CardRemoval()
    {
        gameObject.SetActive(false);

        Debug.Log("Reached end of correct Lerp");
        yield return null;
    }


}
