using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactArchives : CoreGameMode, IInteractable
{
    PlaceableHandler placeableHandler;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        placeableHandler = GameObject.FindWithTag("PlaceableHandler").GetComponent<PlaceableHandler>();
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

    // Update is called once per frame
    void Update()
    {
        if (GameController.GameControl.gameMode == GameMode.ARCHIVE2)
        {

            if (Input.GetKeyDown(KeyCode.Q))
            {
                ArtifactRetrieve();
            }
        }
    }

    //Display a visual wheel of all unlocked artifacts and their stats/effects

    //DEBUGGING
    private void ArtifactRetrieve()
    {
        GameObject newArtifact = placeableHandler.RetrieveArtifact();
        newArtifact.GetComponent<IInteractable>().Interact();
        player.GetComponent<PlayerInteraction>().RearrangeObj(newArtifact);
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
