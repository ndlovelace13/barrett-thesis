using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFileBrowser;
using System.IO;
using System.Linq;

public class MainParser : MonoBehaviour
{
    string jsonFilePath;
    string assetFilePath;

    // Start is called before the first frame update
    void Start()
    {
        //isolate this to a button eventually
        FileSearch();
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

        //Debug.Log(FileBrowser.Success);

        if (!FileBrowser.Result[0].Contains(".json"))
        {
            Debug.Log("Please select a json file");
            StartCoroutine(ShowFileLoadWindow());
        }
        else
        {
            jsonFilePath = FileBrowser.Result[0];
            assetFilePath = FilenameRemove(jsonFilePath);
            Debug.Log(assetFilePath);
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

        //parse each card in the json file, creating a new flashcard and adding it to the deck
        List <Flashcard> cards = DeckSerialize(root, newDeck);
    }

    private List<Flashcard> DeckSerialize(string jsonDeck, Deck bigDeck)
    {
        //create an storage space for all created flashcards
        List<Flashcard> allCards = new List<Flashcard>();
        //parse the json
        Root coreDeck = JsonUtility.FromJson<Root>(jsonDeck);
        //recursive call if decks are nested
        if (coreDeck.children != null)
        {
            Debug.Log("Nested Deck found");
            foreach (var child in coreDeck.children)
            {
                allCards = allCards.Union<Flashcard>(DeckSerialize(JsonUtility.ToJson(child)), bigDeck).ToList<Flashcard>();
            }
        }
        //base case, continue with parsing
        else
        {
            foreach (var model in coreDeck.note_models)
            {

            }
        }
    }
}
