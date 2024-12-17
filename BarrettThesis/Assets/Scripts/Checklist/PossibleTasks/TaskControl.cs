using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "New Card", menuName = "Custom/TaskControl")]
public class TaskControl : MonoBehaviour
{

    public string taskDescription;
    public int tasksComplete;
    public int tasksTotal;
    public bool complete;
    // Start is called before the first frame update
    public TaskControl()
    {

    }

    public virtual bool UpdateTask()
    {
        if (tasksComplete == tasksTotal)
        {
            Debug.Log("Task Complete");
            return true;
        }
        else
        {
            Debug.Log("Task Incomplete");
            return false;
        }
    }
}
