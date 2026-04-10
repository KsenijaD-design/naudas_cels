using UnityEngine;

public class StoryChoiceTimerBarSM : MonoBehaviour
{
    [SerializeField] private RectTransform fillBar;

    private float startWidth;
    private float height;

    private void Awake()
    {
        if (fillBar != null)
        {
            startWidth = fillBar.sizeDelta.x;
            height = fillBar.sizeDelta.y;
        }
    }

    public void ResetBar()
    {
        SetProgress(1f);
    }

    public void SetProgress(float normalized)
    {
        normalized = Mathf.Clamp01(normalized);

        if (fillBar != null)
            fillBar.sizeDelta = new Vector2(startWidth * normalized, height);
    }
}