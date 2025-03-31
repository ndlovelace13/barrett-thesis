using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public enum SprenState
{
    RETRIEVE,
    ROOM,
    WANDER,
    SELECT
}

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
    public float sprenDist;

    //object references
    GameObject archives;
    Vector3 assignedRoom;

    //spren states
    public SprenState currentState;
    

    // Start is called before the first frame update
    void Start()
    {
        scatteredCard.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == SprenState.SELECT)
        {
            transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2f;
            transform.LookAt(Camera.main.transform);
        }
    }

    //call to spawn the spren, begin its starting behavior
    public void SprenSpawn()
    {
        if (archives == null)
            archives = GameObject.FindWithTag("Archives");

        AssignColor();

        Debug.Log("Spren Spawned");

        //send the spren to the archive
        agent.SetDestination(archives.transform.position);
        currentState = SprenState.RETRIEVE;
        StartCoroutine(StateCheck());
        Debug.Log("BRo");
    }

    //assign the spren a new color
    private void AssignColor()
    {
        sprenBody.material.color = new Color(Random.value, Random.value, Random.value, 1f);
    }

    //when the spren reaches the archive, use this to pass on a new card for it to carry around
    public void CardGrab()
    {
        Debug.Log("Grabbing a card");
        //get a new flashcard from the cardqueue
        RoomControl[] allRooms = GameObject.FindObjectsOfType<RoomControl>();
        for (int i = 0; i < allRooms.Length; i++)
        {
            if (allRooms[i].assignedCards.Count > 0)
            {
                heldCard = allRooms[i].assignedCards[0];
                allRooms[i].assignedCards.RemoveAt(0);

                assignedRoom = allRooms[i].sprenPoint.position;
                break;
            }
        }
        Debug.Log("Assigning card #" + heldCard.cardId);

        //assign the flashcard to the spren and fill it
        scatteredCard.SetActive(true);
        cardControl.FillCard(heldCard);

        //change states to room
        currentState = SprenState.ROOM;

        //retrieve a room to be assigned to, must move there before wandering
        agent.SetDestination(assignedRoom);
        
    }

    //coroutine to check if the state has been reached
    IEnumerator StateCheck()
    {
        while (true)
        {
            if (!agent.pathPending && currentState != SprenState.SELECT)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if ((!agent.hasPath || agent.velocity.sqrMagnitude == 0f))
                    {
                        //Debug.Log("shits broke yo");
                        //assign a new location
                        switch (currentState)
                        {
                            case SprenState.RETRIEVE:
                                CardGrab();
                                //retrieve 
                                break;
                            case SprenState.ROOM:
                                StartCoroutine(WanderDestination());
                                break;
                            case SprenState.WANDER:
                                StartCoroutine(WanderDestination());
                                //wander state
                                break;
                            default:
                                Debug.Log("State is not handled, bozo");
                                break;
                        }
                    }
                }
            }

            yield return new WaitForFixedUpdate();
        }
        
    }

    IEnumerator WanderDestination()
    {
        Debug.Log("Wander Reached");
        
        //randomly assign locations for the spren to wander to and change periodically, should be mostly random
        Vector2 newDirection = Random.insideUnitCircle * sprenDist;

        Vector3 actualDir = transform.position + new Vector3(newDirection.x, 0f, newDirection.y);

        NavMeshHit hit;
        NavMesh.SamplePosition(actualDir, out hit, sprenDist, 1);

        agent.SetDestination(hit.position);
        currentState = SprenState.WANDER;

        Debug.Log("New Location assigned");
        yield return null;
    }

    public void SprenSelect()
    {
        StartCoroutine(SelectLerp());
    }

    IEnumerator SelectLerp()
    {
        agent.isStopped = true;
        Vector3 startPos = transform.position;
        Vector3 selectPos;

        float currentTime = 0f;
        while (currentTime < GameController.GameControl.lerpTime)
        {
            selectPos = Camera.main.transform.position + Camera.main.transform.forward * 2f;
            transform.position = Vector3.Lerp(startPos, selectPos, currentTime / GameController.GameControl.lerpTime);

            currentTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        currentState = SprenState.SELECT;
        transform.LookAt(Camera.main.transform);
    }

    public void SprenDeselect()
    {
        //where the spren detaches and goes back down to the ground
        currentState = SprenState.WANDER;
        agent.isStopped = false;
    }

}
