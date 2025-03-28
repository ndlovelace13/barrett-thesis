using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "artifactData", menuName = "ScriptableObjects/Artifacts/MasteryArtifact")]
public class MasteryArtifact : ArtifactData
{
    public override void UnlockCheck()
    {
        if (GameController.SaveData.highestMastery >= unlockThreshold)
            base.UnlockCheck();
        else
            Debug.Log(artifactName + " checked");
    }
}
