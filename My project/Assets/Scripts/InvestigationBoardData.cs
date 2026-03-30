using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Story/Investigation Board Data", fileName = "InvestigationBoardData")]
public class InvestigationBoardData : ScriptableObject
{
    public string boardTitle = "Red Flags";
    [TextArea] public string hintText = "Select every suspicious note, then press Continue.";
    public RedFlagNoteData[] notes;
}

[Serializable]
public class RedFlagNoteData
{
    [TextArea] public string text;
    [Tooltip("True if this note is a correct red flag and should give +3% when selected.")]
    public bool isCorrectRedFlag = false;
}
