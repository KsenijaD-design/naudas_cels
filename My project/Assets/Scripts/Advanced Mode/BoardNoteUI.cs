using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoardNoteUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private TMP_Text label;
    [SerializeField] private NoteAnim noteAnim;
    [SerializeField] private Button button;

    [Header("Data")]
    [TextArea]
    [SerializeField] private string noteText;
    [SerializeField] private bool isCorrectRedFlag;

    [Header("Own Description Image")]
    [SerializeField] private CanvasGroup descriptionCanvas;
    [SerializeField] private float descriptionFadeDuration = 0.25f;

    [Header("Result UI")]
    [SerializeField] private CanvasGroup redOutlineCanvas;
    [SerializeField] private CanvasGroup correctIconCanvas;
    [SerializeField] private CanvasGroup wrongIconCanvas;

    public bool IsSelected => noteAnim != null && noteAnim.IsSelected;
    public bool IsCorrectRedFlag => isCorrectRedFlag;

    private bool resultMode;
    private Coroutine descriptionRoutine;

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();

        if (button != null)
            button.onClick.AddListener(HandleClick);

        RefreshText();
        HideResultUI();
        SetCanvas(descriptionCanvas, 0f, false);
    }

    private void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(HandleClick);
    }

    private void OnValidate()
    {
        RefreshText();
    }

    public void ApplyData(RedFlagNoteData data)
    {
        if (data == null)
            return;

        noteText = data.text;
        isCorrectRedFlag = data.isCorrectRedFlag;

        RefreshText();
    }

    public void ResetState()
    {
        resultMode = false;

        if (noteAnim != null)
        {
            noteAnim.SetInteractionEnabled(true);
            noteAnim.SetSelectionLocked(false);
            noteAnim.ResetState();
        }

        HideResultUI();
        HideDescriptionInstant();
    }

    public void ShowResult()
    {
        resultMode = true;

        bool selected = IsSelected;

        if (noteAnim != null)
        {
            noteAnim.SetInteractionEnabled(true);
            noteAnim.SetSelectionLocked(true);
            noteAnim.SetSelectedScale();
        }

        HideResultUI();

        if (isCorrectRedFlag)
        {
            SetCanvas(redOutlineCanvas, 1f, false);

            if (selected)
                SetCanvas(correctIconCanvas, 1f, false);
            else
                SetCanvas(wrongIconCanvas, 1f, false);
        }
        else
        {
            if (selected)
                SetCanvas(wrongIconCanvas, 1f, false);
        }
    }

    private void HandleClick()
    {
        if (!resultMode)
            return;

        ToggleDescription();
    }

    public void HideDescriptionInstant()
    {
        if (descriptionRoutine != null)
            StopCoroutine(descriptionRoutine);

        SetCanvas(descriptionCanvas, 0f, false);
    }

    private void ToggleDescription()
    {
        if (descriptionCanvas == null)
            return;

        float target = descriptionCanvas.alpha > 0.5f ? 0f : 1f;
        FadeDescription(target);
    }

    private void FadeDescription(float targetAlpha)
    {
        if (descriptionRoutine != null)
            StopCoroutine(descriptionRoutine);

        descriptionRoutine = StartCoroutine(FadeCanvas(descriptionCanvas, targetAlpha));
    }

    private System.Collections.IEnumerator FadeCanvas(CanvasGroup cg, float targetAlpha)
    {
        if (cg == null)
            yield break;

        float startAlpha = cg.alpha;
        float time = 0f;

        cg.interactable = false;
        cg.blocksRaycasts = false;

        while (time < descriptionFadeDuration)
        {
            time += Time.deltaTime;

            float t = Mathf.Clamp01(time / Mathf.Max(0.01f, descriptionFadeDuration));
            t = Mathf.SmoothStep(0f, 1f, t);

            cg.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        cg.alpha = targetAlpha;
        cg.interactable = false;
        cg.blocksRaycasts = false;
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

    private void HideResultUI()
    {
        SetCanvas(redOutlineCanvas, 0f, false);
        SetCanvas(correctIconCanvas, 0f, false);
        SetCanvas(wrongIconCanvas, 0f, false);
    }

    private void SetCanvas(CanvasGroup canvas, float alpha, bool interactable)
    {
        if (canvas == null)
            return;

        canvas.alpha = alpha;
        canvas.interactable = interactable;
        canvas.blocksRaycasts = interactable;
    }
}