using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VisitorHandler : MonoBehaviour
{
    [SerializeField] ObjectPool visitorPool;
    float visitorCooldown;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void VisitorSpawn()
    {
        VisitorCalc();
        StartCoroutine(VisitorSpawning());
    }

    //set the variables here for visitor calculation - TODO!!!
    private void VisitorCalc()
    {
        visitorCooldown = 5f;
        GameController.SaveData.maxVisitors = 2;
    }

    IEnumerator VisitorSpawning()
    {
        float currentCooldown = 0f;
        while (GameController.SaveData.museumOpen)
        {
            //spawn a visitor
            if (currentCooldown > visitorCooldown)
            {
                if (FindObjectsOfType<VisitorBehavior>().Length < GameController.SaveData.maxVisitors)
                {
                    GameObject newVisitor = visitorPool.GetPooledObject();
                    newVisitor.SetActive(true);
                    newVisitor.transform.position = transform.position;
                    newVisitor.GetComponent<VisitorBehavior>().BeginVisit();
                }
                else
                {
                    Debug.Log("Too many visitors");
                }
                currentCooldown = 0f;
            }
            else
                currentCooldown += 1f;
            yield return new WaitForSeconds(1f);
        }
    }
}
