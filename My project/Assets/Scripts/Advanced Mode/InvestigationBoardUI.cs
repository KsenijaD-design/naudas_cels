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
    [SerializeField] private TMPro.TMP_Text continueButtonText;
    [SerializeField] private string checkText = "Pārbaudīt";
    [SerializeField] private string continueText = "Turpināt";
    
    [Header("Reputation")]
    [SerializeField] private float correctValue = 3f;
    [SerializeField] private float wrongValue = 3f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip continueSound;
    [SerializeField] private AudioClip reputationUpSound;
    [SerializeField] private AudioClip reputationDownSound;

    [Header("UI")]
    [SerializeField] private CanvasGroup boardCanvas;
    [SerializeField] private CanvasGroup reputationUI;
    [SerializeField] private float fadeDuration = 0.25f;
    [SerializeField] private float reputationDelay = 3f;

    private enum BoardPhase
    {
        Selecting,
        Reviewing,
        Finished
    }

    private BoardPhase phase;

    private Coroutine boardFadeRoutine;
    private Coroutine reputationFadeRoutine;
    private Coroutine reputationDelayRoutine;

    private System.Action onContinueCallback;
    private bool reputationAlreadyCalculated;

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
        phase = BoardPhase.Selecting;
        reputationAlreadyCalculated = false;

        StopAllUIRoutines();

        if (continueButton != null)
            continueButton.interactable = true;
        if (continueButtonText != null)
            continueButtonText.text = checkText;

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
        if (phase == BoardPhase.Selecting)
        {
            ShowAllResults();
            return;
        }

        if (phase == BoardPhase.Reviewing)
        {
            FinishBoard();
        }
    }

    private void ShowAllResults()
    {
        phase = BoardPhase.Reviewing;
        
        if (continueButtonText != null)
            continueButtonText.text = continueText;
        
        PlaySound(continueSound);

        if (!reputationAlreadyCalculated)
        {
            reputationAlreadyCalculated = true;

            float delta = CalculateReputation();

            if (ReputationSystem.Instance != null)
            {
                if (delta > 0f)
                {
                    ReputationSystem.Instance.AddPercent(delta);
                    PlaySound(reputationUpSound);
                }
                else if (delta < 0f)
                {
                    ReputationSystem.Instance.RemovePercent(-delta);
                    PlaySound(reputationDownSound);
                }
            }
        }

        foreach (var note in notes)
        {
            if (note != null)
                note.ShowResult();
        }
    }

    private void FinishBoard()
    {
        phase = BoardPhase.Finished;

        if (continueButton != null)
            continueButton.interactable = false;

        PlaySound(continueSound);

        HideAllNoteDescriptions();
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
        if (notes == null)
            return;

        foreach (var note in notes)
        {
            if (note != null)
                note.ResetState();
        }
    }

    private void HideAllNoteDescriptions()
    {
        if (notes == null)
            return;

        foreach (var note in notes)
        {
            if (note != null)
                note.HideDescriptionInstant();
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

        cg.interactable = false;
        cg.blocksRaycasts = false;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / Mathf.Max(0.01f, fadeDuration));
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

    private void PlaySound(AudioClip clip)
    {
        if (audioSource == null || clip == null)
            return;

        audioSource.PlayOneShot(clip);
    }
}