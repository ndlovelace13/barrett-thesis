using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParseObjs : MonoBehaviour
{
    
}

[System.Serializable]
public class DeckConfiguration
{
    public string __type__ { get; set; }
    public int answerAction { get; set; }
    public bool autoplay { get; set; }
    public bool buryInterdayLearning { get; set; }
    public string crowdanki_uuid { get; set; }
    public double desiredRetention { get; set; }
    public bool dyn { get; set; }
    public List<object> fsrsWeights { get; set; }
    public string ignoreRevlogsBeforeDate { get; set; }
    public int interdayLearningMix { get; set; }
    public Lapse lapse { get; set; }
    public int maxTaken { get; set; }
    public string name { get; set; }
    public New @new { get; set; }
    public int newGatherPriority { get; set; }
    public int newMix { get; set; }
    public int newPerDayMinimum { get; set; }
    public int newSortOrder { get; set; }
    public int questionAction { get; set; }
    public bool replayq { get; set; }
    public Rev rev { get; set; }
    public int reviewOrder { get; set; }
    public double secondsToShowAnswer { get; set; }
    public double secondsToShowQuestion { get; set; }
    public double sm2Retention { get; set; }
    public bool stopTimerOnAnswer { get; set; }
    public int timer { get; set; }
    public bool waitForAudio { get; set; }
    public string weightSearch { get; set; }
}

[System.Serializable]
public class Fld
{
    public bool collapsed { get; set; }
    public string description { get; set; }
    public bool excludeFromSearch { get; set; }
    public string font { get; set; }
    public object id { get; set; }
    public List<object> media { get; set; }
    public string name { get; set; }
    public int ord { get; set; }
    public bool plainText { get; set; }
    public bool preventDeletion { get; set; }
    public bool rtl { get; set; }
    public int size { get; set; }
    public bool sticky { get; set; }
    public object tag { get; set; }
}

[System.Serializable]
public class Lapse
{
    public List<double> delays { get; set; }
    public int leechAction { get; set; }
    public int leechFails { get; set; }
    public int minInt { get; set; }
    public double mult { get; set; }
}

[System.Serializable]
public class New
{
    public bool bury { get; set; }
    public List<double> delays { get; set; }
    public int initialFactor { get; set; }
    public List<int> ints { get; set; }
    public int order { get; set; }
    public int perDay { get; set; }
}

[System.Serializable]
public class Note
{
    public string __type__ { get; set; }
    public List<string> fields { get; set; }
    public string guid { get; set; }
    public string note_model_uuid { get; set; }
    public List<object> tags { get; set; }
}

[System.Serializable]
public class NoteModel
{
    public string __type__ { get; set; }
    public string crowdanki_uuid { get; set; }
    public string css { get; set; }
    public List<Fld> flds { get; set; }
    public string latexPost { get; set; }
    public string latexPre { get; set; }
    public bool latexsvg { get; set; }
    public string name { get; set; }
    public long originalId { get; set; }
    public List<List<object>> req { get; set; }
    public int sortf { get; set; }
    public List<string> tags { get; set; }
    public List<Tmpl> tmpls { get; set; }
    public int type { get; set; }
    public List<object> vers { get; set; }
}

[System.Serializable]
public class Rev
{
    public bool bury { get; set; }
    public double ease4 { get; set; }
    public double hardFactor { get; set; }
    public double ivlFct { get; set; }
    public int maxIvl { get; set; }
    public int perDay { get; set; }
}

[System.Serializable]
public class Root
{
    public string __type__ { get; set; }
    public List<object> children { get; set; }
    public string crowdanki_uuid { get; set; }
    public string deck_config_uuid { get; set; }
    public List<DeckConfiguration> deck_configurations { get; set; }
    public string desc { get; set; }
    public int dyn { get; set; }
    public int extendNew { get; set; }
    public int extendRev { get; set; }
    public List<string> media_files { get; set; }
    public string name { get; set; }
    public object newLimit { get; set; }
    public object newLimitToday { get; set; }
    public List<NoteModel> note_models { get; set; }
    public List<Note> notes { get; set; }
    public object reviewLimit { get; set; }
    public object reviewLimitToday { get; set; }
}

[System.Serializable]
public class Tmpl
{
    public string afmt { get; set; }
    public string bafmt { get; set; }
    public string bfont { get; set; }
    public string bqfmt { get; set; }
    public int bsize { get; set; }
    public object did { get; set; }
    public object id { get; set; }
    public string name { get; set; }
    public int ord { get; set; }
    public string qfmt { get; set; }
}
