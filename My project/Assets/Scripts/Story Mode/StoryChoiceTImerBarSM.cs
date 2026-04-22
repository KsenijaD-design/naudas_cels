using UnityEngine;

public class StoryChoiceTimerBarSM : MonoBehaviour
{
    [SerializeField] private RectTransform fillBar;
    [SerializeField] private CanvasGroup canvasGroup;

    private float startWidth;
    private float height;

    private void Awake()
    {
        if (fillBar != null)
        {
            startWidth = fillBar.sizeDelta.x;
            height = fillBar.sizeDelta.y;
        }

        if (canvasGroup != null)
            canvasGroup.alpha = 1f;
    }

    public void ResetBar()
    {
        if (fillBar != null)
            fillBar.sizeDelta = new Vector2(startWidth, height);

        if (canvasGroup != null)
            canvasGroup.alpha = 1f;
    }

    public void SetProgress(float normalized)
    {
        normalized = Mathf.Clamp01(normalized);

        if (fillBar != null)
            fillBar.sizeDelta = new Vector2(startWidth * normalized, height);
    }

    public void SetAlpha(float value)
    {
        if (canvasGroup != null)
            canvasGroup.alpha = value;
    }
}