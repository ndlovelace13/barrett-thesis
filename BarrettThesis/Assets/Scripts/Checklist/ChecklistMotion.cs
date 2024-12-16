using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChecklistMotion : ObjectMotion
{
    bool disabled = false;

    [SerializeField] Transform startingPos;
    [SerializeField] Transform inspectPos;
    [SerializeField] Transform offScreen;

    public override void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        held = true;
    }

    public override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Space) && !disabled)
        {
            InspectHandler();
        }

        DisableHandler();

        /*if (GameController.GameControl.gameMode == GameMode.DEFAULT)
        {
            Debug.Log("What the fuck");
        }*/
    }

    private void InspectHandler()
    {
        StopAllCoroutines();
        if (inspecting)
        {
            StartCoroutine(Uninspect());
            GameController.GameControl.gameMode = GameMode.DEFAULT;
        }
        else
        {
            StartCoroutine(Inspect());
            GameController.GameControl.gameMode = GameMode.INSPECTING;
        }
        inspecting = !inspecting;
    }

    private void DisableHandler()
    {
        if (GameController.GameControl.gameMode == GameMode.DEFAULT)
        {
            //lerp checklist back to its normal place
            if (disabled)
            {
                disabled = false;
                StartCoroutine(Enable());
                Debug.Log("Lerping Checklist Up");
            }
        }
        else if (GameController.GameControl.gameMode != GameMode.INSPECTING)
        {
            //lerp checklist down and don't render it
            if (!disabled)
            {
                Debug.Log("Lerping Checklist Down");
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
        Debug.Log("Enabling Checklist");
        GetComponent<ChecklistDisplay>().TaskUpdate();
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
        Debug.Log("Done Enabling Checklist");
    }

}
