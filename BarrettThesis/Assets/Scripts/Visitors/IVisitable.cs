using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVisitable
{
    public float RetrieveHappiness();

    public float VisitTime();

    public float AvgVisitTime();
}
