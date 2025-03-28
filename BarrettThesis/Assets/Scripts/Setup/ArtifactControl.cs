using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactControl : MonoBehaviour
{
    List<ArtifactData> artifactData;
    public List<ArtifactData> unlockQueue;
    bool popupActive = false;


    [Header("Popup Components")]
    [SerializeField] Animator artifactPopup;
    [SerializeField] Image texture;
    [SerializeField] TMP_Text artifactName;
    [SerializeField] TMP_Text artifactDescription;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //call everytime the game starts - check whether the save data has all achievements instantiated
    public void ArtifactRestore()
    {
        artifactData = new List<ArtifactData>();
        artifactData = Resources.LoadAll<ArtifactData>("Artifacts/").ToList<ArtifactData>();
        Debug.Log(artifactData.Count + " artifacts found in files");
        Debug.Log(GameController.SaveData.artifactData.Count + " artifacts found in save data");

        //check whether there is a mismatch in the count of artifacts in save and in files
        if (artifactData.Count == GameController.SaveData.artifactData.Count)
            return;

        foreach (var data in artifactData)
        {
            if (!GameController.SaveData.artifactData.Contains(data))
            {
                GameController.SaveData.artifactData.Add(data);
            }
        }

        Debug.Log(GameController.SaveData.artifactData.Count + " artifacts now found in save data");

        StartCoroutine(ArtifactPopup());
    }

    public void ArtifactUnlock()
    {
        StartCoroutine(ArtifactCheck());
    }

    IEnumerator ArtifactCheck()
    {
        foreach (var data in GameController.SaveData.artifactData)
        {
            if (!data.unlocked)
                data.UnlockCheck();
            yield return new WaitForEndOfFrame();
        }
    }

    //will run as long as the game is active - only executes the animations when something is added to the unlock queue
    IEnumerator ArtifactPopup()
    {
        unlockQueue = new List<ArtifactData>();
        while (true)
        {
            if (unlockQueue.Count > 0 && !popupActive)
            {
                //stop the coroutine from starting any others while a popup is already active
                popupActive = true;

                //retrieve the oldest artifactdata from the queue
                ArtifactData currentData = unlockQueue.First();
                unlockQueue.Remove(currentData);

                //fill the fields of the popup
                FillPopup(currentData);

                //execute animation
                artifactPopup.SetTrigger("activate");
            }
            yield return new WaitForFixedUpdate();
        }
    }

    public void FillPopup(ArtifactData data)
    {
        texture.sprite = data.texture;
        artifactName.text = data.artifactName;
        artifactDescription.text = data.effectDescription;
    }

    public void PopupComplete()
    {
        popupActive = false;
    }
}
