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

    public List<Outline> RetrieveOutline()
    {
        if (!string.IsNullOrEmpty(outlineTag))
        {
            List<Outline> outlines = new List<Outline>();
            GameObject[] associatedObjs = GameObject.FindGameObjectsWithTag(outlineTag);
            foreach (GameObject obj in associatedObjs)
            {
                outlines.Add(obj.GetComponent<Outline>());
            }
            return outlines;
        }
        else
            return null;
    }
}
