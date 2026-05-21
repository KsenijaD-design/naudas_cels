using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NewspaperUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Buttons")]
    [SerializeField] private Button[] articleButtons;

    [Header("Descriptions")]
    [SerializeField] private CanvasGroup[] descriptionGroups;

    [Header("Continue")]
    [SerializeField] private Button continueButton;

    [Header("Fade")]
    [SerializeField] private float descriptionFadeTime = 0.25f;

    private bool[] readArticles;
    private Coroutine[] fadeRoutines;
    private Action onContinue;

    private void Awake()
    {
        readArticles = new bool[articleButtons.Length];
        fadeRoutines = new Coroutine[descriptionGroups.Length];

        for (int i = 0; i < articleButtons.Length; i++)
        {
            int index = i;
            articleButtons[i].onClick.RemoveAllListeners();
            articleButtons[i].onClick.AddListener(() => OpenArticle(index));
        }

        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(Continue);

        continueButton.interactable = false;

        HideAllDescriptionsInstant();
        SetCanvas(canvasGroup, 0f, false);
    }

    public void Show(Action callback)
    {
        onContinue = callback;

        for (int i = 0; i < readArticles.Length; i++)
            readArticles[i] = false;

        continueButton.interactable = false;

        HideAllDescriptionsInstant();
        SetCanvas(canvasGroup, 1f, true);
    }

    public void HideInstant()
    {
        HideAllDescriptionsInstant();
        SetCanvas(canvasGroup, 0f, false);
    }

    private void OpenArticle(int index)
    {
        if (index < 0 || index >= descriptionGroups.Length)
            return;

        bool wasVisible = descriptionGroups[index] != null &&
                          descriptionGroups[index].alpha > 0.5f;

        for (int i = 0; i < descriptionGroups.Length; i++)
        {
            if (i == index && !wasVisible)
                FadeDescription(i, 1f);
            else
                FadeDescription(i, 0f);
        }

        if (!wasVisible)
            readArticles[index] = true;

        CheckAllRead();
    }

    private void FadeDescription(int index, float targetAlpha)
    {
        if (descriptionGroups[index] == null)
            return;

        if (fadeRoutines[index] != null)
            StopCoroutine(fadeRoutines[index]);

        fadeRoutines[index] = StartCoroutine(FadeCanvas(descriptionGroups[index], targetAlpha));
    }

    private IEnumerator FadeCanvas(CanvasGroup group, float targetAlpha)
    {
        float startAlpha = group.alpha;
        float time = 0f;

        group.interactable = false;
        group.blocksRaycasts = false;

        while (time < descriptionFadeTime)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / descriptionFadeTime);
            t = Mathf.SmoothStep(0f, 1f, t);

            group.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        group.alpha = targetAlpha;
        group.interactable = targetAlpha > 0.9f;
        group.blocksRaycasts = targetAlpha > 0.9f;
    }

    private void HideAllDescriptionsInstant()
    {
        for (int i = 0; i < descriptionGroups.Length; i++)
            SetCanvas(descriptionGroups[i], 0f, false);
    }

    private void CheckAllRead()
    {
        for (int i = 0; i < readArticles.Length; i++)
        {
            if (!readArticles[i])
            {
                continueButton.interactable = false;
                return;
            }
        }

        continueButton.interactable = true;
    }

    private void Continue()
    {
        HideAllDescriptionsInstant();
        SetCanvas(canvasGroup, 0f, false);
        onContinue?.Invoke();
    }

    private void SetCanvas(CanvasGroup group, float alpha, bool interactable)
    {
        if (group == null)
            return;

        group.alpha = alpha;
        group.interactable = interactable;
        group.blocksRaycasts = interactable;
    }
}