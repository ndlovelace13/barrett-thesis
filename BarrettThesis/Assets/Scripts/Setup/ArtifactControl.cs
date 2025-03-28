using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArtifactControl : MonoBehaviour
{
    [SerializeField] List<ArtifactData> artifactData;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //call everytime the game starts - check whether the save data has all achievements instantiated
    public void ArtifactRestore()
    {
        artifactData = new List<ArtifactData>();
        artifactData = Resources.LoadAll<ArtifactData>("Artifacts/").ToList<ArtifactData>();
        Debug.Log(artifactData.Count + " artifacts found in files");
        Debug.Log(GameController.SaveData.artifactData.Count + " artifacts found in save data");

        //check whether there is a mismatch in the count of artifacts in save and in files
        if (artifactData.Count == GameController.SaveData.artifactData.Count)
            return;

        foreach (var data in artifactData)
        {
            if (!GameController.SaveData.artifactData.Contains(data))
            {
                GameController.SaveData.artifactData.Add(data);
            }
        }

        Debug.Log(GameController.SaveData.artifactData.Count + " artifacts now found in save data");
    }

    public void ArtifactUnlock()
    {
        StartCoroutine(ArtifactCheck());
    }

    IEnumerator ArtifactCheck()
    {
        foreach (var data in GameController.SaveData.artifactData)
        {
            if (!data.unlocked)
                data.UnlockCheck();
            yield return new WaitForEndOfFrame();
        }
    }
}
