using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChecklistMotion : MonoBehaviour
{

    [SerializeField] PlayerMovement player;

    float currentY = 0f;
    float maxY = 0.15f;
    float minY = -0.15f;
    bool up = true;
    bool inspecting = false;
    bool disabled = false;

    [SerializeField] Transform startingPos;
    [SerializeField] Transform inspectPos;
    [SerializeField] Transform offScreen;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player.moving && !inspecting)
        {
            if (up)
            {
                if (currentY < maxY)
                {
                    currentY += 1 * Time.deltaTime;
                    transform.localPosition = transform.localPosition + Vector3.up * Time.deltaTime * 0.1f;
                }
                else
                {
                    currentY = maxY;
                    up = false;
                }  
            }
            else
            {
                if (currentY > minY)
                {
                    currentY -= 1 * Time.deltaTime;
                    transform.localPosition = transform.localPosition + Vector3.down * Time.deltaTime * 0.1f;
                }
                else
                {
                    currentY = minY;
                    up = true;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && !disabled)
        {
            InspectHandler();
        }

        DisableHandler();
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
            }
        }
        else if (GameController.GameControl.gameMode != GameMode.INSPECTING)
        {
            //lerp checklist down and don't render it
            if (!disabled)
            {
                StopAllCoroutines();
                disabled = true;
                StartCoroutine(Disable());
            }
        }
        

    }

    IEnumerator Inspect()
    {
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
        yield return null;
        
    }

    IEnumerator Enable()
    {
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
        yield return null;
    }
}
