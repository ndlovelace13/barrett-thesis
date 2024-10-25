using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Linq;

[System.Serializable]
public class NoteType
{
    public string id;
    public List<string> fields;
    public List<int> front;
    public List<int> back;

    //instantiate a NoteType obj to only store the necessary information from the parsed data for our purposes
    public NoteType(JSONNode coreModel)
    {
        fields = new List<string>();
        front = new List<int>();
        back = new List<int>();

        id = coreModel["crowdanki_uuid"].Value;
        
        //get all field names from fld obj
        foreach (var field in coreModel["flds"].Values)
        {
            Debug.Log(field["name"].Value);
            fields.Add(field["name"].Value);
        }

        JSONNode tmpl = null;

        foreach (var temp in coreModel["tmpls"].Values)
        {
            if (tmpl == null)
                tmpl = temp;
            else
                break;
        }

        

        //Debug.Log(tmpl["qfmt"]);

        //check the tmpls for which fields are on the question and which are on the answers
        string[] delims = { "{{", "}}" };
        string[] question = tmpl["qfmt"].Value.Split(delims, System.StringSplitOptions.RemoveEmptyEntries);
        string[] answer = tmpl["afmt"].Value.Split(delims, System.StringSplitOptions.RemoveEmptyEntries);

        //foreach (var bro in question)
            //Debug.Log(bro);
        

        for (int i = 0; i < fields.Count; i++)
        {
            if (question.Contains(fields[i]))
                front.Add(i);
            else if (answer.Contains(fields[i]))
                back.Add(i);
        }

        Debug.Log("Note Model parsing completed: Front - " + front.Count + " Back - " + back.Count);
    }
}

[System.Serializable]
public class Deck
{
    //must create separate lists for the dict in order to save
    public List<string> noteTypeIndex;
    public List<NoteType> noteTypes;
    //dict for actual referencing
    Dictionary<string, NoteType> noteTypeDict;
    public List<Flashcard> cards;
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
        noteTypeIndex = new List<string>();
        noteTypes = new List<NoteType>();
        cards = new List<Flashcard>();
        ogJson = jsonPath;
        mediaPath = media;
    }

    //Dictionary can't be stored to json using JsonUtility, call this function whenever you need to restore the dictionary
    //for example, when creating a deck for the first time or loading one from a json file
    public void noteDictTranslate()
    {
        noteTypeDict = new Dictionary<string, NoteType>();
        for (int i = 0; i < noteTypeIndex.Count; i++)
        {
            noteTypeDict.Add(noteTypeIndex[i], noteTypes[i]);
        }

        Debug.Log("Dict created - Count: " + noteTypeDict.Count);
    }
}
