using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "New Card", menuName = "Custom/TaskControl")]
public class TaskControl : MonoBehaviour
{

    public string taskDescription;
    public int tasksComplete;
    public int tasksTotal;
    // Start is called before the first frame update
    public TaskControl()
    {

    }

    public virtual void UpdateTask()
    {
        Debug.Log("Task Updated");
    }
}
