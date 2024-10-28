using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFileBrowser;
using System.IO;
using System.Linq;
using SimpleJSON;

public class MainParser : MonoBehaviour
{
    string jsonFilePath;
    string assetFilePath;

    // Start is called before the first frame update
    void Start()
    {
        //isolate this to a button eventually
        //FileSearch();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FileSearch()
    {
        FileBrowser.SetDefaultFilter(".json");
        FileBrowser.SetFilters(true);
        FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe", ".jpg", ".png", ".mp3", ".mp4");
        StartCoroutine(ShowFileLoadWindow());
    }

    IEnumerator ShowFileLoadWindow()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, null, null, "Select a File", "Create Deck");

        if (!FileBrowser.Success)
            GameObject.FindWithTag("TitleUI").GetComponent<TitleBehavior>().Cancel();
        else
        {
            if (!FileBrowser.Result[0].Contains(".json"))
            {
                Debug.Log("Please select a json file");
                StartCoroutine(ShowFileLoadWindow());
            }
            else
            {
                jsonFilePath = FileBrowser.Result[0];
                assetFilePath = FilenameRemove(jsonFilePath);
                Debug.Log("Assets stored at: " + assetFilePath);
                DeckCreate();
                GameObject.FindWithTag("TitleUI").GetComponent<TitleBehavior>().ChooseSave();
            }
        } 
    }

    //replace the filename with the folder that the json references
    private string FilenameRemove(string filepath)
    {
        string[] pathSplit = filepath.Split("\\");
        string actualFile = pathSplit[pathSplit.Length - 1];
        return filepath.Replace(actualFile, "media\\");
    }

    private void DeckCreate()
    {
        //Instantiate a new Deck obj
        Deck newDeck = new Deck(jsonFilePath, assetFilePath);

        //store the root json to a string, pass to DeckSerialize
        string root = File.ReadAllText(jsonFilePath);
        var rootDeck = JSON.Parse(root);
        //Debug.Log(root);

        //parse each card in the json file, creating a new flashcard and adding it to the deck
        List <Flashcard> cards = DeckSerialize(rootDeck, newDeck);
        newDeck.cards = cards;
        Debug.Log("Deck note models: " + newDeck.noteTypes.Count);
        Debug.Log("Card Count: " + newDeck.cards.Count);
        Debug.Log("Parse successful");
        GameController.SaveData.currentDeck = newDeck;
        //GameController.SaveData.testCard = newDeck.cards[0];
        //SaveHandler.SaveSystem.SaveGame();
    }

    private List<Flashcard> DeckSerialize(JSONNode jsonDeck, Deck bigDeck)
    {
        //create an storage space for all created flashcards
        List<Flashcard> allCards = new List<Flashcard>();
        //parse the json
        //var coreDeck = JSON.Parse(jsonDeck);
        Debug.Log(jsonDeck["children"].Count);
        //recursive call if decks are nested
        if (jsonDeck["children"].Count > 0)
        {
            Debug.Log("Nested Deck found");
            foreach (var child in jsonDeck["children"].Values)
            {
                allCards = allCards.Union<Flashcard>(DeckSerialize(child, bigDeck)).ToList<Flashcard>();
            }
        }
        //base case, continue with parsing
        else
        {
            JSONNode noteModels = jsonDeck["note_models"];
            Debug.Log(jsonDeck["crowdanki_uuid"]);

            foreach (var model in noteModels.Values)
            {
                //create a new NoteType, store in the deck lists, then translate to dict once all have been loaded
                NoteType newType = new NoteType(model);
                bigDeck.noteTypeIndex.Add(newType.id);
                bigDeck.noteTypes.Add(newType);
            }

            //create the notedict
            bigDeck.noteDictTranslate();

            allCards = NoteParse(jsonDeck["notes"]);
        }

        Debug.Log("Card Count at end of Parse: " + allCards.Count);
        return allCards;
    }

    private List<Flashcard> NoteParse(JSONNode jsonNode)
    {
        List<Flashcard> newCards = new List<Flashcard>();

        //parse each notecard in the deck and create an associated flashcard
        foreach (var note in jsonNode.Values)
        {
            Flashcard newNote = new Flashcard(note);
            newCards.Add(newNote);
        }

        Debug.Log("Card Count at end of note parse: " + newCards.Count);
        return newCards;
    }
        
}
