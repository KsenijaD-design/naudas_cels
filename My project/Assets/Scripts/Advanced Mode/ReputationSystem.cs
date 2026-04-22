using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReputationSystem : MonoBehaviour
{
    public static ReputationSystem Instance { get; private set; }

    [Header("Value")]
    [SerializeField, Range(0f, 100f)] private float startReputation = 50f;
    [SerializeField] private float minReputation = 0f;
    [SerializeField] private float maxReputation = 100f;

    [Header("UI Refs")]
    [SerializeField] private RectTransform reputationFill;
    [SerializeField] private Image reputationFillImage;

    [Header("Bar Width")]
    [Tooltip("Width of the bar when reputation is exactly 50%.")]
    [SerializeField] private float centerWidth = 300f;

    [Tooltip("Full width of the bar when reputation reaches 25% or 75%.")]
    [SerializeField] private float fullWidth = 600f;

    [Header("Colors")]
    [SerializeField] private Color colorAt25 = Color.red;
    [SerializeField] private Color colorAt50 = Color.yellow;
    [SerializeField] private Color colorAt75 = Color.green;

    [Header("Animation")]
    [SerializeField] private float uiLerpSpeed = 6f;

    public float CurrentReputation { get; private set; }

    private float displayedWidth;
    private Color displayedColor;
    private Coroutine animateRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        CurrentReputation = Mathf.Clamp(startReputation, minReputation, maxReputation);

        displayedWidth = GetWidthForReputation(CurrentReputation);
        displayedColor = GetColorForReputation(CurrentReputation);

        ApplyUIInstant(displayedWidth, displayedColor);
    }

    public void AddPercent(float value)
    {
        SetReputation(CurrentReputation + value);
    }

    public void RemovePercent(float value)
    {
        SetReputation(CurrentReputation - Mathf.Abs(value));
    }

    public void SetReputation(float value)
    {
        CurrentReputation = Mathf.Clamp(value, minReputation, maxReputation);

        float targetWidth = GetWidthForReputation(CurrentReputation);
        Color targetColor = GetColorForReputation(CurrentReputation);

        if (animateRoutine != null)
            StopCoroutine(animateRoutine);

        animateRoutine = StartCoroutine(AnimateUI(targetWidth, targetColor));
    }

    public void RefreshUI()
    {
        float targetWidth = GetWidthForReputation(CurrentReputation);
        Color targetColor = GetColorForReputation(CurrentReputation);

        if (animateRoutine != null)
            StopCoroutine(animateRoutine);

        animateRoutine = StartCoroutine(AnimateUI(targetWidth, targetColor));
    }

    private float GetWidthForReputation(float reputation)
    {
        if (reputation <= 50f)
        {
            float t = Mathf.InverseLerp(50f, 25f, reputation);
            return Mathf.Lerp(centerWidth, fullWidth, t);
        }

        float tUpper = Mathf.InverseLerp(50f, 75f, reputation);
        return Mathf.Lerp(centerWidth, fullWidth, tUpper);
    }

    private Color GetColorForReputation(float reputation)
    {
        if (reputation <= 50f)
        {
            float t = Mathf.InverseLerp(25f, 50f, reputation);
            return Color.Lerp(colorAt25, colorAt50, t);
        }

        float tUpper = Mathf.InverseLerp(50f, 75f, reputation);
        return Color.Lerp(colorAt50, colorAt75, tUpper);
    }

    private IEnumerator AnimateUI(float targetWidth, Color targetColor)
    {
        while (Mathf.Abs(displayedWidth - targetWidth) > 0.1f ||
               Vector4.Distance(displayedColor, targetColor) > 0.01f)
        {
            displayedWidth = Mathf.Lerp(displayedWidth, targetWidth, Time.deltaTime * uiLerpSpeed);
            displayedColor = Color.Lerp(displayedColor, targetColor, Time.deltaTime * uiLerpSpeed);

            ApplyUIInstant(displayedWidth, displayedColor);
            yield return null;
        }

        displayedWidth = targetWidth;
        displayedColor = targetColor;
        ApplyUIInstant(displayedWidth, displayedColor);

        animateRoutine = null;
    }

    private void ApplyUIInstant(float width, Color color)
    {
        if (reputationFill != null)
        {
            Vector2 size = reputationFill.sizeDelta;
            size.x = width;
            reputationFill.sizeDelta = size;
        }

        if (reputationFillImage != null)
            reputationFillImage.color = color;
    }
}