using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ArtifactArchives : CoreGameMode, IInteractable
{
    [SerializeField] GameObject artifactPrefab;
    int displayedIndex = 0;
    GameObject artifact;
    bool retrievable = false;

    [Header("Archive UI")]
    [SerializeField] Canvas artifactArchiveUI;
    [SerializeField] TMP_Text artifactName;
    [SerializeField] TMP_Text artifactCriteria;
    [SerializeField] TMP_Text unlockDay;
    [SerializeField] TMP_Text overallProgress;
    [SerializeField] TMP_Text retrievePrompt;

    PlaceableHandler placeableHandler;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        placeableHandler = GameObject.FindWithTag("PlaceableHandler").GetComponent<PlaceableHandler>();
        artifactArchiveUI.enabled = false;
        gameMode = GameMode.ARCHIVE2;
    }

    public override bool Interact()
    {
        if (player.GetComponent<PlayerInteraction>().heldObj == null)
        {
            base.Interact();
            return true;
        }
        else
            return false;
    }

    public override bool CancelInteract()
    {
        if (artifact.transform.parent == null)
            Destroy(artifact);
        artifactArchiveUI.enabled = false;
        return base.CancelInteract();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.GameControl.gameMode == GameMode.ARCHIVE2)
        {

            if (Input.GetKeyDown(KeyCode.Q) && retrievable)
            {
                ArtifactRetrieve();
            }

            //movement between displayed archives
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                ArchiveIncrement(-1);
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                ArchiveIncrement(1);
        }
    }

    protected override void PostCameraShift()
    {
        StartCoroutine(ArtifactDisplay());
    }

    IEnumerator ArtifactDisplay()
    {
        artifactArchiveUI.enabled = true;

        //create the obj
        artifact = Instantiate(artifactPrefab);

        //set card grid a certain dist from the user
        artifact.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 3f;

        //in case need to set rotation
        artifact.transform.LookAt(Camera.main.transform);

        UpdateArtifact();

        yield return null;
    }

    private void ArchiveIncrement(int direction)
    {
        if (displayedIndex == 0 && direction < 0)
            displayedIndex = GameController.SaveData.artifactData.Count - 1;
        else if (displayedIndex == GameController.SaveData.artifactData.Count - 1 && direction > 0)
            displayedIndex = 0;
        else
            displayedIndex += direction;

        UpdateArtifact();
    }

    private void UpdateArtifact()
    {
        //store the current artifactData in the artifact
        ArtifactData currentData = GameController.SaveData.artifactData[displayedIndex];
        artifact.GetComponent<Artifact>().data = currentData;

        //fill the UI elements to reflect whether it is discovered or not
        artifactCriteria.text = currentData.effectDescription;
        
        //assign the material only if it is unlocked
        if (currentData.unlocked)
        {
            artifactName.text = currentData.artifactName;
            artifact.GetComponent<Artifact>().RestoreArtifact();
            unlockDay.text = "Discovered on Day " + currentData.unlockDate;

            //gray out here
            if (!currentData.onDisplay)
            {
                retrievePrompt.enabled = true;
                retrievable = true;
            }
            else
            {
                retrievePrompt.enabled = false;
                retrievable = false;
            }
        }
        else
        {
            retrievePrompt.enabled = false;
            retrievable = false;
            artifactName.text = "Undiscovered Artifact";
            unlockDay.text = "Keep Studying to Unlock";

            //apply some placeholder material here in place
        }

        //update the persistent UI elements
        overallProgress.text = GameController.SaveData.unlockedArtifacts + "/" + GameController.SaveData.artifactData.Count + " Discovered";
            
    }



    //Display a visual wheel of all unlocked artifacts and their stats/effects

    //DEBUGGING
    private void ArtifactRetrieve()
    {
        //outmode this function, retrieving the currently shown artifact instead
        //GameObject newArtifact = placeableHandler.RetrieveArtifact();

        artifact.GetComponent<IInteractable>().Interact();
        player.GetComponent<PlayerInteraction>().RearrangeObj(artifact);
        CancelInteract();
    }

    public override string GetPrompt()
    {
        //Change after Debugging is complete
        if (player.GetComponent<PlayerInteraction>().heldObj != null)
            return "Already holding an item";
        else
            return "Press E to claim a new artifact";
    }
}
