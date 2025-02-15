using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MatchScanner : ObjectMotion
{
    LayerMask layer;

    bool disabled = false;

    [SerializeField] Transform startingPos;
    [SerializeField] Transform offScreen;

    [SerializeField] MeshRenderer scannedPainting;
    [SerializeField] TMP_Text scannedPrompt;

    Texture placeholder;

    Flashcard scannedCard;

    // Start is called before the first frame update
    void Start()
    {
        layer = LayerMask.NameToLayer("cardLayer");
        placeholder = scannedPainting.material.mainTexture;
    }

    public void FixedUpdate()
    {
        if (!disabled)
            StartCoroutine(ScanCheck());

        DisableHandler();
    }


    IEnumerator ScanCheck()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 100f, layer))
        {
            //if a card is detected, display it to the screen of the scanner
            scannedCard = hit.collider.transform.root.GetComponent<ScatteredCard>().ReportCard();
        }
        else
            scannedCard = null;

        //apply to the scanner
        if (scannedCard != null)
        {
            if (scannedCard.useCustom)
                scannedPainting.material.mainTexture = SaveHandler.SaveSystem.GetPainting(scannedCard.customArt);
            NoteType noteInfo = GameController.SaveData.currentDeck.dictRetrieve(scannedCard.noteId);
            scannedPrompt.text = scannedCard.fields[noteInfo.matchPromptField];
        }
        else
        {
            scannedPainting.material.mainTexture = placeholder;
            scannedPrompt.text = "No Archive Detected";
        }

        yield return null;
    }

    private void DisableHandler()
    {
        if (GameController.GameControl.gameMode == GameMode.MATCHING)
        {
            //lerp checklist back to its normal place
            if (disabled)
            {
                disabled = false;
                StartCoroutine(Enable());
                Debug.Log("Lerping Scanner Up");
            }
        }
        else if (GameController.GameControl.gameMode != GameMode.MATCHING)
        {
            //lerp checklist down and don't render it
            if (!disabled)
            {
                Debug.Log("Lerping Scanner Down");
                StopAllCoroutines();
                disabled = true;
                StartCoroutine(Disable());
            }
        }


    }

    IEnumerator Disable()
    {
        Vector3 startLoc = transform.localPosition;
        float timer = 0f;
        while (timer < 1f)
        {
            transform.localPosition = Vector3.Lerp(startLoc, offScreen.localPosition, timer);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            if (!disabled)
                yield break;
        }
        transform.localPosition = offScreen.localPosition;
        GetComponent<MeshRenderer>().enabled = false;
        held = false;
        yield return null;

    }

    IEnumerator Enable()
    {
        Debug.Log("Enabling Scanner");
        //GetComponent<ChecklistDisplay>().TaskUpdate();
        Vector3 startLoc = transform.localPosition;
        GetComponent<MeshRenderer>().enabled = true;
        float timer = 0f;
        while (timer < .25f)
        {
            transform.localPosition = Vector3.Lerp(startLoc, startingPos.localPosition, timer / .25f);
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            if (disabled)
                yield break;
        }
        transform.localPosition = startingPos.localPosition;
        held = true;
        yield return null;
        Debug.Log("Done Enabling Scanner");
    }
}
