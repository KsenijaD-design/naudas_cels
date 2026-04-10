using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Story/Investigation Board Data", fileName = "InvestigationBoardData")]
public class InvestigationBoardData : ScriptableObject
{
    public string boardTitle = "Red Flags";
    [TextArea] public string hintText = "Select suspicious notes and press Continue.";
    public RedFlagNoteData[] notes;
}

[Serializable]
public class RedFlagNoteData
{
    [TextArea] public string text;
    public bool isCorrectRedFlag = false;
}
