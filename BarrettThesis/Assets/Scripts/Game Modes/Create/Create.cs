using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Create : CoreGameMode, IInteractable
{

    [SerializeField] PlaceableHandler allPlaceable;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        gameMode = GameMode.CREATING;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Interact()
    {
        base.Interact();
    }

    public override string GetPrompt()
    {
        return "Press E to Retrieve a New Object";
    }

    protected override void PostCameraShift()
    {
        base.PostCameraShift();
        SpawnNewObj();
    }

    //Debugging
    private void SpawnNewObj()
    {
        CancelInteract();
        GameObject newObj = allPlaceable.RandomPlaceable();
        newObj.GetComponent<IInteractable>().Interact();
        player.GetComponent<PlayerInteraction>().RearrangeObj(newObj);
        player.GetComponent<PlayerInteraction>().isInteracting = false;
    }
}
