using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardMotion : MonoBehaviour
{

    bool selected = true;
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
        float currentTime = 0f;

        Quaternion startingRot = transform.rotation;
        Quaternion endingRot = new Quaternion(startingRot.x, 360f - startingRot.y, startingRot.z, startingRot.w);

        while (currentTime < 0.25f)
        {
            transform.rotation = Quaternion.Lerp(startingRot, endingRot, currentTime);
            yield return new WaitForEndOfFrame();
            currentTime += Time.deltaTime;
        }
    }
}
