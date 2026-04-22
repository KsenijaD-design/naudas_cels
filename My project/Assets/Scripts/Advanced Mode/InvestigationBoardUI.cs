using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvestigationBoardUI : MonoBehaviour
{
    [Header("Notes")]
    [SerializeField] private List<BoardNoteUI> notes = new List<BoardNoteUI>();

    [Header("Buttons")]
    [SerializeField] private Button continueButton;

    [Header("Reputation")]
    [SerializeField] private float correctValue = 3f;
    [SerializeField] private float wrongValue = 3f;

    [Header("UI")]
    [SerializeField] private CanvasGroup boardCanvas;
    [SerializeField] private CanvasGroup reputationUI;
    [SerializeField] private float fadeDuration = 0.25f;
    [SerializeField] private float reputationDelay = 3f;

    private Coroutine boardFadeRoutine;
    private Coroutine reputationFadeRoutine;
    private Coroutine reputationDelayRoutine;

    private System.Action onContinueCallback;

    private void Awake()
    {
        if (continueButton != null)
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(OnContinuePressed);
        }

        SetCanvasInstant(boardCanvas, 0f, false);
        SetCanvasInstant(reputationUI, 0f, false);
    }

    public void Show(InvestigationBoardData data, System.Action onContinue)
    {
        onContinueCallback = onContinue;

        StopAllUIRoutines();

        if (data != null && data.notes != null)
        {
            for (int i = 0; i < notes.Count && i < data.notes.Length; i++)
            {
                if (notes[i] == null || data.notes[i] == null)
                    continue;

                notes[i].ApplyData(data.notes[i]);
            }
        }

        ResetBoard();

        FadeBoardTo(1f, true);
        FadeReputationTo(1f, false);
    }

    public void Hide()
    {
        FadeBoardTo(0f, false);
    }

    public void HideInstant()
    {
        StopAllUIRoutines();
        SetCanvasInstant(boardCanvas, 0f, false);

    }

    private void OnContinuePressed()
    {
        float delta = CalculateReputation();

        if (ReputationSystem.Instance != null)
        {
            if (delta >= 0f)
                ReputationSystem.Instance.AddPercent(delta);
            else
                ReputationSystem.Instance.RemovePercent(-delta);
        }
        
        FadeBoardTo(0f, false);
        
        onContinueCallback?.Invoke();
        
        if (reputationDelayRoutine != null)
            StopCoroutine(reputationDelayRoutine);

        reputationDelayRoutine = StartCoroutine(ReputationDelayThenFade());
    }

    private IEnumerator ReputationDelayThenFade()
    {
        yield return new WaitForSeconds(reputationDelay);
        FadeReputationTo(0f, false);
        reputationDelayRoutine = null;
    }

    private void ResetBoard()
    {
        foreach (var note in notes)
        {
            if (note == null)
                continue;

            note.ResetState();
        }
    }

    private float CalculateReputation()
    {
        int selectedCount = 0;
        int totalNotes = 0;
        float total = 0f;

        foreach (var note in notes)
        {
            if (note == null)
                continue;

            totalNotes++;

            if (!note.IsSelected)
                continue;

            selectedCount++;

            if (note.IsCorrectRedFlag)
                total += correctValue;
            else
                total -= wrongValue;
        }

        if (selectedCount == totalNotes && totalNotes > 0)
            return -wrongValue * 2f;

        if (selectedCount == 0)
            return -wrongValue;

        return total;
    }

    private void FadeBoardTo(float targetAlpha, bool interactableAtEnd)
    {
        if (boardFadeRoutine != null)
            StopCoroutine(boardFadeRoutine);

        boardFadeRoutine = StartCoroutine(FadeCanvas(boardCanvas, targetAlpha, interactableAtEnd));
    }

    private void FadeReputationTo(float targetAlpha, bool interactableAtEnd)
    {
        if (reputationFadeRoutine != null)
            StopCoroutine(reputationFadeRoutine);

        reputationFadeRoutine = StartCoroutine(FadeCanvas(reputationUI, targetAlpha, interactableAtEnd));
    }

    private IEnumerator FadeCanvas(CanvasGroup cg, float targetAlpha, bool interactableAtEnd)
    {
        if (cg == null)
            yield break;

        float startAlpha = cg.alpha;
        float time = 0f;

        if (targetAlpha < startAlpha)
        {
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / fadeDuration);
            t = Mathf.SmoothStep(0f, 1f, t);

            cg.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        cg.alpha = targetAlpha;
        cg.interactable = interactableAtEnd;
        cg.blocksRaycasts = interactableAtEnd;
    }

    private void SetCanvasInstant(CanvasGroup cg, float alpha, bool interactable)
    {
        if (cg == null)
            return;

        cg.alpha = alpha;
        cg.interactable = interactable;
        cg.blocksRaycasts = interactable;
    }

    private void StopAllUIRoutines()
    {
        if (boardFadeRoutine != null)
            StopCoroutine(boardFadeRoutine);

        if (reputationFadeRoutine != null)
            StopCoroutine(reputationFadeRoutine);

        if (reputationDelayRoutine != null)
            StopCoroutine(reputationDelayRoutine);

        boardFadeRoutine = null;
        reputationFadeRoutine = null;
        reputationDelayRoutine = null;
    }
}