using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "artifactData", menuName = "ScriptableObjects/Artifacts/ExpansionsArtifact")]
public class ExpansionsArtifact : ArtifactData
{
    public override void UnlockCheck()
    {
        if (GameController.SaveData.additionalRooms > unlockThreshold)
            base.UnlockCheck();
        else
            Debug.Log(artifactName + " checked");
    }
}
