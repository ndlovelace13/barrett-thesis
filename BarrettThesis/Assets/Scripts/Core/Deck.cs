using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NoteType
{
    public string id;
    public List<string> fields;
    public List<int> front;
    public List<int> back;

    //instantiate a NoteType obj to only store the necessary information from the parsed data for our purposes
    public NoteType(NoteModel coreModel)
    {
        fields = new List<string>();
        front = new List<int>();
        back = new List<int>();

        id = coreModel.crowdanki_uuid;
        
        //get all field names from fld obj
        foreach (var field in coreModel.flds)
        {
            fields.Add(field.name);
        }

        //check the tmpls for which fields are on the question and which are on the answers
    }
}

public class Deck
{
    public List<Flashcard> cards;
    public Dictionary<string, NoteType>
    string ogJson;
    string mediaPath;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Deck(string jsonPath, string media)
    {
        cards = new List<Flashcard>();
        ogJson = jsonPath;
        mediaPath = media;
    }
}
