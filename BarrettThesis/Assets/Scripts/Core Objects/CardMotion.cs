using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardMotion : MonoBehaviour
{

    public bool selected = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (selected)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                StartCoroutine(CardFlip());
            }
        }
    }

    IEnumerator CardFlip()
    {
        Debug.Log("Flipping Card");
        float currentTime = 0f;

        Vector3 startingRot = transform.eulerAngles;
        Vector3 endingRot = startingRot + 180f * Vector3.up;

        while (currentTime < 0.5f)
        {
            transform.eulerAngles = Vector3.Lerp(startingRot, endingRot, currentTime / 0.5f);
            yield return new WaitForEndOfFrame();
            currentTime += Time.deltaTime;
        }
        transform.eulerAngles = endingRot;
    }
}
