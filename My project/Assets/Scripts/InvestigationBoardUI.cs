using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InvestigationBoardUI : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Text")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text hintText;

    [Header("Manual Notes")]
    [SerializeField] private BoardNoteUI[] notes;

    [Header("Buttons")]
    [SerializeField] private Button continueButton;

    [Header("Scoring")]
    [SerializeField] private float correctRewardPercent = 3f;
    [SerializeField] private float wrongPenaltyPercent = 3f;

    private InvestigationBoardData currentData;
    private Action onContinue;

    private void Reset()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (continueButton != null)
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(HandleContinue);
        }

        HideInstant();
    }

    public void Show(InvestigationBoardData data, Action continueCallback)
    {
        currentData = data;
        onContinue = continueCallback;

        if (titleText != null)
            titleText.text = data != null ? data.boardTitle : "Board";

        if (hintText != null)
            hintText.text = data != null ? data.hintText : string.Empty;

        ResetAllNotes();
        SetVisible(true);
    }

    public void HideInstant()
    {
        ResetAllNotes();
        SetVisible(false);
    }

    private void HandleContinue()
    {
        if (ReputationSystem.Instance != null && notes != null)
        {
            for (int i = 0; i < notes.Length; i++)
            {
                BoardNoteUI note = notes[i];
                if (note == null || !note.IsSelected)
                    continue;

                if (note.IsCorrectRedFlag)
                    ReputationSystem.Instance.AddPercent(correctRewardPercent);
                else
                    ReputationSystem.Instance.RemovePercent(wrongPenaltyPercent);
            }
        }

        SetVisible(false);

        Action callback = onContinue;
        onContinue = null;
        callback?.Invoke();
    }

    private void ResetAllNotes()
    {
        if (notes == null) return;

        for (int i = 0; i < notes.Length; i++)
        {
            if (notes[i] != null)
                notes[i].ResetState();
        }
    }

    private void SetVisible(bool visible)
    {
        gameObject.SetActive(true);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }
    }
}
