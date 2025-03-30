using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "tutorialItem", menuName = "ScriptableObjects/TutorialItem")]
public class TutorialItem : ScriptableObject
{
    public string key;
    public string charName;
    [TextArea] public string[] dialogue;
    public string outlineTag;

    public Outline RetrieveOutline()
    {
        GameObject associatedObj = GameObject.FindWithTag(outlineTag);
        if (associatedObj == null)
            return null;
        else
            return associatedObj.GetComponent<Outline>();
    }
}
