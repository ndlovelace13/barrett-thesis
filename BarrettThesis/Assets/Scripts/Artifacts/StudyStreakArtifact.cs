using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "artifactData", menuName = "ScriptableObjects/Artifacts/StudyStreakArtifact")]
public class StudyStreakArtifact : ArtifactData
{
    public override void UnlockCheck()
    {
        if (GameController.SaveData.highestStudyStreak >= unlockThreshold)
            base.UnlockCheck();
        else
            Debug.Log(artifactName + " checked");
    }
}
