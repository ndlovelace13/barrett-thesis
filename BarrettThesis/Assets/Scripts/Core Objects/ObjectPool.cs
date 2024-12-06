using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] GameObject instance;
    [SerializeField] int objectNum;

    List<GameObject> pooledObjects;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Awake()
    {
        pooledObjects = new List<GameObject>();
        for (int i = 0; i < objectNum; i++)
        {
            GameObject newInstance = Instantiate(instance);
            newInstance.SetActive(false);
            pooledObjects.Add(newInstance);
        }
    }

    public GameObject GetPooledObject()
    {
        foreach (GameObject obj in pooledObjects)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }
        Debug.Log("No objects to return");
        return null;
    }
}
