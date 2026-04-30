using TMPro;
using UnityEngine;

public class BoardNoteUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private TMP_Text label;
    [SerializeField] private NoteAnim noteAnim;

    [Header("Data")]
    [TextArea]
    [SerializeField] private string noteText;
    [SerializeField] private bool isCorrectRedFlag = false;

    public bool IsSelected => noteAnim != null && noteAnim.IsSelected;
    public bool IsCorrectRedFlag => isCorrectRedFlag;

    private void Awake()
    {
        RefreshText();
    }

    private void OnValidate()
    {
        RefreshText();
    }
    
    public void ShowResult()
    {
        if (noteAnim != null)
            noteAnim.ShowResult(IsCorrectRedFlag);
    }
    
    public void ApplyData(RedFlagNoteData data)
    {
        if (data == null) return;

        noteText = data.text;
        isCorrectRedFlag = data.isCorrectRedFlag;

        RefreshText();
    }

    public void ResetState()
    {
        if (noteAnim != null)
            noteAnim.ResetState();
    }

    public void SetNoteText(string newText)
    {
        noteText = newText;
        RefreshText();
    }

    private void RefreshText()
    {
        if (label != null)
            label.text = noteText;
    }
}