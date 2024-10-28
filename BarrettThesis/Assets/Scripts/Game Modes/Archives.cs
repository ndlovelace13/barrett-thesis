using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archives : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            GameController.GameControl.lockPlayer = true;
            StartCoroutine(ArchiveCam());
        }
        if (Input.GetKeyDown(KeyCode.Escape) && GameController.GameControl.lockPlayer)
        {
            GameController.GameControl.lockPlayer = false;
        }
    }

    IEnumerator ArchiveCam()
    {
        float currentTime = 0f;
        Quaternion startingRot = Camera.main.transform.rotation;
        Quaternion endingRot = new Quaternion(0f, startingRot.y, 0f, startingRot.w);
        //lerp the camera to straight on
        while (currentTime < 1f)
        {
            Camera.main.transform.rotation = Quaternion.Lerp(startingRot, endingRot, currentTime);
            yield return new WaitForEndOfFrame();
            currentTime += Time.deltaTime;
        }
        Camera.main.transform.rotation = endingRot;

        yield return null;
    }
}
