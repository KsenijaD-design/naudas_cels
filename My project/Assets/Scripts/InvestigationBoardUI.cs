using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InvestigationBoardUI : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.25f;

    [Header("Optional Text")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text hintText;

    [Header("Manual Notes")]
    [SerializeField] private BoardNoteUI[] notes;

    [Header("Buttons")]
    [SerializeField] private Button continueButton;

    [Header("Scoring")]
    [SerializeField] private float correctRewardPercent = 3f;
    [SerializeField] private float wrongPenaltyPercent = 3f;

    private Action onContinue;
    private Coroutine fadeRoutine;

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
        onContinue = continueCallback;
        ApplyBoardData(data);
        ResetAllNotes();

        gameObject.SetActive(true);
        StartFade(true);
    }

    public void Hide()
    {
        StartFade(false);
    }

    public void HideInstant()
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        gameObject.SetActive(true);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        ResetAllNotes();
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

        Hide();

        Action callback = onContinue;
        onContinue = null;
        callback?.Invoke();
    }

    private void ApplyBoardData(InvestigationBoardData data)
    {
        if (titleText != null && data != null)
            titleText.text = data.boardTitle;

        if (hintText != null && data != null)
            hintText.text = data.hintText;

        if (data == null || data.notes == null || notes == null)
            return;

        for (int i = 0; i < notes.Length; i++)
        {
            if (notes[i] == null)
                continue;

            if (i < data.notes.Length)
            {
                notes[i].gameObject.SetActive(true);
                notes[i].ApplyData(data.notes[i]);
            }
            else
            {
                notes[i].gameObject.SetActive(false);
            }
        }
    }

    private void ResetAllNotes()
    {
        if (notes == null) return;

        for (int i = 0; i < notes.Length; i++)
        {
            if (notes[i] != null && notes[i].gameObject.activeSelf)
                notes[i].ResetState();
        }
    }

    private void StartFade(bool show)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeRoutine(show));
    }

    private IEnumerator FadeRoutine(bool show)
    {
        if (canvasGroup == null)
            yield break;

        gameObject.SetActive(true);

        float start = canvasGroup.alpha;
        float target = show ? 1f : 0f;
        float time = 0f;
        float duration = Mathf.Max(0.01f, fadeDuration);

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, target, time / duration);
            yield return null;
        }

        canvasGroup.alpha = target;
        canvasGroup.interactable = show;
        canvasGroup.blocksRaycasts = show;
        fadeRoutine = null;
    }
}
