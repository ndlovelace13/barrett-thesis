using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;


public class VisitorReport : ObjectMotion
{
    [Header("Report Components")]
    [SerializeField] GameObject VisitorInfo;
    [SerializeField] TMP_Text dayCounter;
    [SerializeField] TMP_Text visitorCount;
    [SerializeField] TMP_Text timeAway;
    [SerializeField] TMP_Text totalPayout;

    bool disabled = false;

    [Header("Lerping Locations")]
    [SerializeField] Transform startingPos;
    [SerializeField] Transform offScreen;
    [SerializeField] Transform inspectPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public override void Update()
    {
        if (GameController.GameControl.gameMode == GameMode.REPORT && Input.GetKeyDown(KeyCode.Escape) && !inspecting)
        {
            ReportDisable();
        }
        if (GameController.GameControl.gameMode == GameMode.REPORT && Input.GetKeyDown(KeyCode.Space))
        {
            InspectHandler();
        }
        base.Update();
    }

    void FixedUpdate()
    {
        DisableHandler();
    }

    private void InspectHandler()
    {
        StopAllCoroutines();
        if (inspecting)
        {
            StartCoroutine(Uninspect());
            //GameController.GameControl.gameMode = GameMode.REPORT;
        }
        else
        {
            StartCoroutine(Inspect());
            //GameController.GameControl.gameMode = GameMode.INSPECTING;
        }
        inspecting = !inspecting;
    }

    public void FillReport(TimeSpan timeaway, int totalVisits, int totalEarnings)
    {
        dayCounter.text = "Day " + GameController.SaveData.dayIndex;
        timeAway.text = "Time Open: " + timeaway.ToString(@"hh\:mm");
        visitorCount.text = "No. of Visitors: " + totalVisits;
        totalPayout.text = "Donations Received: " + ((float)(totalEarnings / 100f)).ToString("C2");

        //activate the report
        StartCoroutine(ReportPopup());
    }

    IEnumerator ReportPopup()
    {
        GameController.GameControl.gameMode = GameMode.REPORT;
        GameObject.FindFirstObjectByType<TutorialControl>().CheckTutorial("firstVisitorReport");
        yield return null;
    }

    public void ReportDisable()
    {
        GameController.GameControl.gameMode = GameMode.DEFAULT;
    }

    private void DisableHandler()
    {
        if (GameController.GameControl.gameMode == GameMode.REPORT)
        {
            //lerp checklist back to its normal place
            if (disabled)
            {
                disabled = false;
                StartCoroutine(Enable());
                Debug.Log("Lerping Report Up");
            }
        }
        else if (GameController.GameControl.gameMode != GameMode.REPORT)
        {
            //lerp checklist down and don't render it
            if (!disabled)
            {
                Debug.Log("Lerping Report Down");
                StopAllCoroutines();
                disabled = true;
                StartCoroutine(Disable());
            }
        }


    }

    IEnumerator Inspect()
    {
        Debug.Log("Inspecting");
        Vector3 startingPos = transform.position;
        Quaternion startingRot = transform.rotation;

        while (transform.position != inspectPos.position && transform.rotation != inspectPos.rotation)
        {
            transform.position = Vector3.MoveTowards(transform.position, inspectPos.position, 2 * Time.deltaTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, inspectPos.rotation, 100 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        transform.position = inspectPos.position;
        transform.rotation = inspectPos.rotation;

        yield return null;
    }

    IEnumerator Uninspect()
    {
        Debug.Log("Canceling Inspect");
        while (transform.position != startingPos.position && transform.rotation != startingPos.rotation)
        {
            transform.position = Vector3.MoveTowards(transform.position, startingPos.position, 2 * Time.deltaTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, startingPos.rotation, 100 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        transform.position = startingPos.position;
        transform.rotation = startingPos.rotation;

        yield return null;
    }

    IEnumerator Disable()
    {
        Vector3 startLoc = transform.localPosition;
        float timer = 0f;
        while (timer < 1f)
        {
            transform.localPosition = Vector3.Lerp(startLoc, offScreen.localPosition, timer);
            yield return new WaitForFixedUpdate();
            timer += Time.fixedDeltaTime;
            if (!disabled)
                yield break;
        }
        transform.localPosition = offScreen.localPosition;
        //GetComponent<MeshRenderer>().enabled = false;
        held = false;
        yield return null;

    }

    IEnumerator Enable()
    {
        Debug.Log("Enabling Report");


        //GetComponent<ChecklistDisplay>().TaskUpdate();
        Vector3 startLoc = transform.localPosition;
        //GetComponent<MeshRenderer>().enabled = true;
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
