using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class StoryManagerSM : MonoBehaviour
{
    [Header("Video")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private StoryNodeSM startNode;
    [SerializeField] private GameManagerSM gameManager;

    [Header("Choice UI")]
    [SerializeField] private CanvasGroup twoChoiceCanvas;
    [SerializeField] private CanvasGroup threeChoiceCanvas;

    [SerializeField] private ChoiceButtonUISM[] twoChoiceButtons;
    [SerializeField] private ChoiceButtonUISM[] threeChoiceButtons;

    [Header("Timer")]
    [SerializeField] private CanvasGroup timerCanvas;
    [SerializeField] private StoryChoiceTimerBarSM timerBar;

    [Header("Fade")]
    [SerializeField] private float fadeSpeed = 6f;

    private StoryNodeSM currentNode;
    public StoryNodeSM CurrentNode => currentNode;
    
    private Coroutine timerRoutine;

    private float twoChoiceTarget;
    private float threeChoiceTarget;
    private float timerTarget;

    private bool choiceShown;
    private bool choicePicked;

    private void Start()
    {
        SetCanvas(twoChoiceCanvas, 0f);
        SetCanvas(threeChoiceCanvas, 0f);
        SetCanvas(timerCanvas, 0f);

        if (timerBar != null)
            timerBar.ResetBar();

        PlayNode(startNode);
    }

    private void Update()
    {
        Fade(twoChoiceCanvas, twoChoiceTarget);
        Fade(threeChoiceCanvas, threeChoiceTarget);
        Fade(timerCanvas, timerTarget);

        if (currentNode != null &&
            currentNode.isChoiceNode &&
            !choiceShown &&
            videoPlayer != null &&
            videoPlayer.clip != null)
        {
            double remaining = videoPlayer.length - videoPlayer.time;

            if (remaining <= currentNode.showChoicesBeforeEnd)
                ShowChoices();
        }
    }

    public void PlayNode(StoryNodeSM node)
    {
        if (node == null)
        {
            Debug.Log("Story ended.");
            return;
        }

        currentNode = node;
        choiceShown = false;
        choicePicked = false;

        twoChoiceTarget = 0f;
        threeChoiceTarget = 0f;
        timerTarget = 0f;

        if (timerRoutine != null)
        {
            StopCoroutine(timerRoutine);
            timerRoutine = null;
        }

        if (timerBar != null)
            timerBar.ResetBar();

        if (videoPlayer == null)
        {
            Debug.LogError("StoryManagerSM: VideoPlayer is missing.");
            return;
        }

        videoPlayer.loopPointReached -= OnVideoFinished;
        videoPlayer.Stop();
        videoPlayer.clip = node.videoClip;
        videoPlayer.loopPointReached += OnVideoFinished;
        videoPlayer.Play();

        if (gameManager != null)
            gameManager.SetGameplayMode();
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= OnVideoFinished;

        if (currentNode == null)
            return;

        if (!currentNode.isChoiceNode)
        {
            PlayNode(currentNode.nextNode);
            return;
        }

        if (!choiceShown)
            ShowChoices();
    }

    private void ShowChoices()
    {
        if (currentNode == null || currentNode.choices == null || currentNode.choices.Length == 0)
        {
            Debug.LogWarning("StoryManagerSM: choice node has no choices.");
            return;
        }

        choiceShown = true;

        if (gameManager != null)
            gameManager.SetStoryChoiceMode();

        if (currentNode.choices.Length == 2)
        {
            twoChoiceTarget = 1f;
            threeChoiceTarget = 0f;
            Setup(twoChoiceButtons);
        }
        else if (currentNode.choices.Length == 3)
        {
            threeChoiceTarget = 1f;
            twoChoiceTarget = 0f;
            Setup(threeChoiceButtons);
        }
        else
        {
            Debug.LogWarning("StoryManagerSM supports only 2 or 3 choices.");
            return;
        }

        if (currentNode.useTimer)
        {
            timerTarget = 1f;

            if (timerBar != null)
                timerBar.ResetBar();

            if (timerRoutine != null)
                StopCoroutine(timerRoutine);

            timerRoutine = StartCoroutine(ChoiceTimer());
        }
        else
        {
            timerTarget = 0f;

            if (timerBar != null)
                timerBar.ResetBar();
        }
    }

    private void Setup(ChoiceButtonUISM[] buttons)
    {
        if (buttons == null)
            return;

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null)
                continue;

            if (i < currentNode.choices.Length)
            {
                int index = i;
                buttons[i].SetActive(true);
                buttons[i].Setup(currentNode.choices[i].text, () => Pick(index));
            }
            else
            {
                buttons[i].SetActive(false);
            }
        }
    }

    private void Pick(int index)
    {
        if (choicePicked)
            return;

        if (currentNode == null || currentNode.choices == null || index < 0 || index >= currentNode.choices.Length)
        {
            Debug.LogWarning("StoryManagerSM: invalid choice index.");
            return;
        }

        choicePicked = true;
        HideChoices();

        if (timerRoutine != null)
        {
            StopCoroutine(timerRoutine);
            timerRoutine = null;
        }

        if (timerBar != null)
            timerBar.SetProgress(0f);

        if (gameManager != null)
            gameManager.SetGameplayMode();

        PlayNode(currentNode.choices[index].nextNode);
    }

    private void HideChoices()
    {
        twoChoiceTarget = 0f;
        threeChoiceTarget = 0f;
        timerTarget = 0f;
    }

    private IEnumerator ChoiceTimer()
    {
        float duration = Mathf.Max(0.01f, currentNode.choiceTime);
        float t = duration;

        while (t > 0f)
        {
            if (choicePicked)
                yield break;

            t -= Time.deltaTime;

            float normalized = Mathf.Clamp01(t / duration);

            if (timerBar != null)
                timerBar.SetProgress(normalized);

            yield return null;
        }

        if (timerBar != null)
            timerBar.SetProgress(0f);

        int index = Mathf.Clamp(currentNode.defaultChoiceIndex, 0, currentNode.choices.Length - 1);
        Pick(index);
    }

    private void Fade(CanvasGroup cg, float target)
    {
        if (cg == null)
            return;

        cg.alpha = Mathf.MoveTowards(cg.alpha, target, fadeSpeed * Time.deltaTime);
        cg.interactable = cg.alpha > 0.9f;
        cg.blocksRaycasts = cg.alpha > 0.1f;
    }

    private void SetCanvas(CanvasGroup cg, float value)
    {
        if (cg == null)
            return;

        cg.alpha = value;
        cg.interactable = value > 0.9f;
        cg.blocksRaycasts = value > 0.1f;
    }
}