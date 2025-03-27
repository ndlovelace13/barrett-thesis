using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScannerRelay : MonoBehaviour
{
    [SerializeField] MatchScanner scannerControl;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FillAnswers()
    {
        scannerControl.AnswerFill();
    }
}
