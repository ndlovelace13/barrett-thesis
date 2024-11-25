using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChecklistItem : MonoBehaviour
{
    [SerializeField] Image checkBox;
    [SerializeField] TMP_Text taskText;

    public TaskControl taskType;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateItem()
    {
        taskType.UpdateTask();
        Debug.Log("Checklist Item Updated");
        taskText.text = taskType.taskDescription + "(" + taskType.tasksComplete + "/" + taskType.tasksTotal + ")";
    }
}
