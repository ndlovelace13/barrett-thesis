using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChecklistMotion : MonoBehaviour
{

    [SerializeField] PlayerMovement player;

    float currentY = 0f;
    float maxY = 0.2f;
    float minY = -0.2f;
    bool up = true;
    bool inspecting = false;

    [SerializeField] Transform startingPos;
    [SerializeField] Transform inspectPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player.moving && inspecting)
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (inspecting)
            {
                StartCoroutine(Uninspect());
            }
            else
            {
                StartCoroutine(Inspect());
            }
            inspecting = !inspecting;
        }
    }

    IEnumerator Inspect()
    {
        
        yield return null;
    }

    IEnumerator Uninspect()
    {
        yield return null;
    }
}
