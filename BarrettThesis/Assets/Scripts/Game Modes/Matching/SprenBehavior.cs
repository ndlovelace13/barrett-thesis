using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SprenBehavior : MonoBehaviour
{
    [Header("Spren Prefab Elements")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] MeshRenderer sprenBody;
    [SerializeField] Animator sprenAnim;
    [SerializeField] GameObject scatteredCard;
    [SerializeField] ScatteredCard cardControl;

    [Header("Core Data")]
    public Flashcard heldCard;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //call to spawn the spren, begin its starting behavior
    public void SprenSpawn()
    {
        AssignColor();

        //send the spren to the archive
        Debug.Log("Spren Spawned");
    }

    //assign the spren a new color
    private void AssignColor()
    {
        sprenBody.material.color = new Color(Random.value, Random.value, Random.value, 1f);
    }

    //when the spren reaches the archive, use this to pass on a new card for it to carry around
    public void CardGrab(Flashcard grabbedCard)
    {
        heldCard = grabbedCard;
    }
}
