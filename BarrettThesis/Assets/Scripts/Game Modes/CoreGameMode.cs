using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreGameMode : MonoBehaviour, IInteractable
{
    public GameMode gameMode;

    protected GameObject player;
    protected PlayerCam cam;

    [SerializeField] protected Transform playerLocation;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        player = GameObject.FindWithTag("Player");
        cam = Camera.main.GetComponent<PlayerCam>();
        GetComponent<Outline>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Interact()
    {
        GameController.GameControl.lockPlayer = true;
        GameController.GameControl.gameMode = gameMode;
        StartCoroutine(CameraShift());

    }

    public virtual void CancelInteract()
    {
        GameController.GameControl.lockPlayer = false;
        GameController.GameControl.gameMode = GameMode.DEFAULT;
    }

    public IEnumerator CameraShift()
    {
        float currentTime = 0f;
        float duration = 0.5f;
        float camStartX = cam.xRotation;
        float camStartY = cam.yRotation;

        Vector3 startingPos = player.transform.position;
        Quaternion startingRot = player.transform.rotation;
        //Quaternion startingRot = Camera.main.transform.rotation;
        //Quaternion endingRot = new Quaternion(0f, startingRot.y, 0f, startingRot.w);
        //lerp the camera to straight on
        while (currentTime < duration)
        {
            player.transform.position = Vector3.Lerp(startingPos, playerLocation.position, currentTime / duration);
            player.transform.rotation = Quaternion.Lerp(startingRot, playerLocation.rotation, currentTime / duration);
            cam.xRotation = Mathf.Lerp(camStartX, playerLocation.rotation.eulerAngles.x, currentTime / duration);
            cam.yRotation = Mathf.Lerp(camStartY, playerLocation.rotation.eulerAngles.y, currentTime / duration);
            //Camera.main.transform.rotation = Quaternion.Lerp(startingRot, endingRot, currentTime / 0.3f);
            yield return new WaitForFixedUpdate();
            currentTime += Time.fixedDeltaTime;
        }
        //Camera.main.transform.rotation = endingRot;
        player.transform.position = playerLocation.position;
        player.transform.rotation = playerLocation.rotation;
        cam.xRotation = playerLocation.rotation.eulerAngles.x;
        cam.yRotation = playerLocation.rotation.eulerAngles.y;

        PostCameraShift();
        yield return null;
    }

    protected virtual void PostCameraShift()
    {
        Debug.Log("Activating PostCamera Function");
    }

    public virtual string GetPrompt()
    {
        return "Game Mode Prompt Not Handled";
    }

}
