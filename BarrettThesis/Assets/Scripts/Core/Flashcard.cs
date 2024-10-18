using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class Flashcard
{
    public int cardId;
    public List<string> fields;
    public List<int> imgFields;
    public List<int> audioFields;
    public string noteId;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Flashcard(JSONNode noteNode)
    {
        fields = new List<string>();
        imgFields = new List<int>();
        audioFields = new List<int>();
        noteId = noteNode["note_model_uuid"].Value;

        int i = 0;
        foreach (var field in noteNode["fields"].Values)
        {
            string finalField = field;
            //if field contains img, isolate the img address and store index to imgFields
            if (finalField.Contains("<img"))
            {
                string[] splitPath = finalField.Split("\"");
                finalField = splitPath[1];
                imgFields.Add(i);
            }
            //if field contains sound, isolate the sound address and store index to audioFields
            else if (finalField.Contains("[sound:"))
            {
                string[] splitPath = finalField.Split(":");
                finalField = splitPath[1].Substring(0, splitPath[1].Length - 2);
                audioFields.Add(i);
            }

            fields.Add(finalField);
            i++;
        }
        
        Debug.Log("Card created: " + imgFields.Count + " " + audioFields.Count);
    }
}
