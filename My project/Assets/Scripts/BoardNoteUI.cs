using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoardNoteUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text label;
    [SerializeField] private GameObject threadVisual;

    [Header("Data")]
    [TextArea]
    [SerializeField] private string noteText;
    [SerializeField] private bool isCorrectRedFlag = false;

    public bool IsSelected { get; private set; }
    public bool IsCorrectRedFlag => isCorrectRedFlag;

    private void Awake()
    {
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(ToggleSelected);
        }

        RefreshText();
        ResetState();
    }

    public void SetNoteText(string value)
    {
        noteText = value;
        RefreshText();
    }

    public void ResetState()
    {
        IsSelected = false;
        RefreshVisual();
    }

    public void ToggleSelected()
    {
        IsSelected = !IsSelected;
        RefreshVisual();
    }

    private void RefreshText()
    {
        if (label != null)
            label.text = noteText;
    }

    private void RefreshVisual()
    {
        if (threadVisual != null)
            threadVisual.SetActive(IsSelected);
    }
}