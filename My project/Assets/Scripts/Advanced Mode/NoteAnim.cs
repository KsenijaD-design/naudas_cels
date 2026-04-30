using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class NoteAnim : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler
{
    [Header("Refs")]
    [SerializeField] private RectTransform target;
    [SerializeField] private CanvasGroup highlightCanvasGroup;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip noteClickSound;
    [SerializeField] private AudioClip noteSelectSound;
    [SerializeField] private AudioClip noteDeselectSound;

    [Header("Scale Settings")]
    [SerializeField] private float baseScale = 0.30f;
    [SerializeField] private float hoverPeakScale = 0.345f;
    [SerializeField] private float hoverSettleScale = 0.32f;

    [Header("Timing")]
    [SerializeField] private float hoverUpTime = 0.18f;
    [SerializeField] private float hoverDownTime = 0.22f;
    [SerializeField] private float exitTime = 0.25f;

    [Header("Highlight")]
    [SerializeField] private float highlightFadeTime = 0.25f;
    
    [Header("Result Colors")]
    [SerializeField] private Color correctColor = Color.green;
    [SerializeField] private Color wrongColor = Color.red;
    [SerializeField] private UnityEngine.UI.Image highlightImage;

    private bool isHovered;
    private bool isSelected;

    private Coroutine scaleRoutine;
    private Coroutine highlightRoutine;

    public bool IsSelected => isSelected;

    private void Awake()
    {
        if (target == null)
            target = transform as RectTransform;

        if (target != null)
            target.localScale = Vector3.one * baseScale;

        if (highlightCanvasGroup != null)
        {
            highlightCanvasGroup.alpha = 0f;
            highlightCanvasGroup.interactable = false;
            highlightCanvasGroup.blocksRaycasts = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        StartScaleRoutine(HoverSequence());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;

        if (!isSelected)
            StartScaleRoutine(ScaleTo(baseScale, exitTime));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PlaySound(noteClickSound);

        isSelected = !isSelected;

        if (isSelected)
        {
            PlaySound(noteSelectSound);
            StartHighlightRoutine(FadeHighlightTo(1f));

            if (!isHovered)
                StartScaleRoutine(ScaleTo(hoverSettleScale, hoverDownTime));
        }
        else
        {
            PlaySound(noteDeselectSound);
            StartHighlightRoutine(FadeHighlightTo(0f));

            if (!isHovered)
                StartScaleRoutine(ScaleTo(baseScale, exitTime));
        }
    }

    public void ResetState()
    {
        isSelected = false;

        if (highlightRoutine != null)
            StopCoroutine(highlightRoutine);

        if (highlightCanvasGroup != null)
            highlightCanvasGroup.alpha = 0f;

        if (!isHovered)
            StartScaleRoutine(ScaleTo(baseScale, exitTime));
    }

    private void StartScaleRoutine(IEnumerator routine)
    {
        if (scaleRoutine != null)
            StopCoroutine(scaleRoutine);

        scaleRoutine = StartCoroutine(routine);
    }

    private void StartHighlightRoutine(IEnumerator routine)
    {
        if (highlightRoutine != null)
            StopCoroutine(highlightRoutine);

        highlightRoutine = StartCoroutine(routine);
    }

    private IEnumerator HoverSequence()
    {
        yield return ScaleTo(hoverPeakScale, hoverUpTime);
        yield return ScaleTo(hoverSettleScale, hoverDownTime);
    }

    private IEnumerator ScaleTo(float targetScale, float duration)
    {
        if (target == null)
            yield break;

        float startScale = target.localScale.x;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            t = Mathf.SmoothStep(0f, 1f, t);

            float scale = Mathf.Lerp(startScale, targetScale, t);
            target.localScale = Vector3.one * scale;

            yield return null;
        }

        target.localScale = Vector3.one * targetScale;
    }

    private IEnumerator FadeHighlightTo(float targetAlpha)
    {
        if (highlightCanvasGroup == null)
            yield break;

        float start = highlightCanvasGroup.alpha;
        float time = 0f;

        while (time < highlightFadeTime)
        {
            time += Time.deltaTime;
            float t = time / highlightFadeTime;
            t = Mathf.SmoothStep(0f, 1f, t);

            highlightCanvasGroup.alpha = Mathf.Lerp(start, targetAlpha, t);
            yield return null;
        }

        highlightCanvasGroup.alpha = targetAlpha;
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource == null || clip == null)
            return;

        audioSource.PlayOneShot(clip);
    }
    
    public void ShowResult(bool isCorrect)
    {
        isSelected = true;

        if (highlightImage != null)
            highlightImage.color = isCorrect ? correctColor : wrongColor;

        if (highlightCanvasGroup != null)
            highlightCanvasGroup.alpha = 1f;

        StartScaleRoutine(ScaleTo(hoverSettleScale, hoverDownTime));
    }
}