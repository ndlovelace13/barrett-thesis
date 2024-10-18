using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashcard
{
    public int cardId;
    public string[] fields;
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

    public Flashcard(int arrayLength, List<string> foundFields, string noteType)
    {
        fields = new string[arrayLength];
        imgFields = new List<int>();
        audioFields = new List<int>();
        noteId = noteType;

        //store all fields to the field array
        for (int i = 0; i < arrayLength; i++)
        {
            fields[i] = foundFields[i];
            
            //if field contains img, isolate the img address and store index to imgFields
            if (fields[i].Contains("<img"))
            {
                string[] splitPath = fields[i].Split("\"");
                fields[i] = splitPath[1];
                imgFields.Add(i);
            }
            //if field contains sound, isolate the sound address and store index to audioFields
            else if (fields[i].Contains("[sound:"))
            {
                string[] splitPath = fields[i].Split(":");
                fields[i] = splitPath[1].Substring(0, splitPath[1].Length - 2);
                audioFields.Add(i);
            }
        }
    }
}
