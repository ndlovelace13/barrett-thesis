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

        Quaternion startingRot = transform.rotation;
        Quaternion endingRot = new Quaternion(startingRot.x, startingRot.y + 180f, startingRot.z, startingRot.w);

        while (currentTime < 1f)
        {
            transform.rotation = Quaternion.Lerp(startingRot, endingRot, currentTime / 1f);
            yield return new WaitForEndOfFrame();
            currentTime += Time.deltaTime;
        }
    }
}
