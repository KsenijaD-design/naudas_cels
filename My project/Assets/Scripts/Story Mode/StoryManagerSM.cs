using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class StoryManagerSM : MonoBehaviour
{
    [Header("Video")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private StoryNodeSM startNode;
    [SerializeField] private GameManagerSM gameManager;

    [Header("Main Menu")]
    [SerializeField] private string mainMenuSceneName = "Main Menu";

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
    
    [Header("Episode Progress")]
    [SerializeField] private string episodeId = "episode_1S";

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
            ReturnToMainMenu();
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
            return;

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
        {
            ReturnToMainMenu();
            return;
        }

        if (!currentNode.isChoiceNode)
        {
            if (currentNode.nextNode != null)
                PlayNode(currentNode.nextNode);
            else
                ReturnToMainMenu();

            return;
        }

        if (!choiceShown)
            ShowChoices();
    }

    private void ShowChoices()
    {
        if (currentNode == null || currentNode.choices == null || currentNode.choices.Length == 0)
        {
            ReturnToMainMenu();
            return;
        }

        choiceShown = true;

        if (gameManager != null)
            gameManager.SetStoryChoiceMode();

        if (currentNode.choices.Length == 2)
        {
            if (twoChoiceCanvas != null)
                twoChoiceCanvas.gameObject.SetActive(true);

            if (threeChoiceCanvas != null)
                threeChoiceCanvas.gameObject.SetActive(false);

            twoChoiceTarget = 1f;
            threeChoiceTarget = 0f;

            Setup(twoChoiceButtons);
        }
        else if (currentNode.choices.Length == 3)
        {
            if (threeChoiceCanvas != null)
                threeChoiceCanvas.gameObject.SetActive(true);

            if (twoChoiceCanvas != null)
                twoChoiceCanvas.gameObject.SetActive(false);

            threeChoiceTarget = 1f;
            twoChoiceTarget = 0f;

            Setup(threeChoiceButtons);
        }
        else
        {
            ReturnToMainMenu();
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
            ReturnToMainMenu();
            return;
        }

        choicePicked = true;
        HideChoices();

        if (timerRoutine != null)
        {
            StopCoroutine(timerRoutine);
            timerRoutine = null;
        }

        StartCoroutine(HideTimerAndContinue(currentNode.choices[index].nextNode));
    }

    private void HideChoices()
    {
        twoChoiceTarget = 0f;
        threeChoiceTarget = 0f;
        timerTarget = 0f;

        if (twoChoiceCanvas != null)
            twoChoiceCanvas.gameObject.SetActive(false);

        if (threeChoiceCanvas != null)
            threeChoiceCanvas.gameObject.SetActive(false);
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

        int defaultIndex = Mathf.Clamp(currentNode.defaultChoiceIndex, 0, currentNode.choices.Length - 1);
        StartCoroutine(HideTimerAndContinue(currentNode.choices[defaultIndex].nextNode));
    }

    private IEnumerator HideTimerAndContinue(StoryNodeSM nextNode)
    {
        choicePicked = true;

        if (timerCanvas != null)
            timerCanvas.alpha = 0f;

        if (timerBar != null)
            timerBar.SetAlpha(0f);

        yield return null;

        if (timerBar != null)
            timerBar.ResetBar();

        if (gameManager != null)
            gameManager.SetGameplayMode();

        PlayNode(nextNode);
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

    private void ReturnToMainMenu()
    {
        EpisodeProgress.MarkCompleted(episodeId);

        Time.timeScale = 1f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (!string.IsNullOrEmpty(mainMenuSceneName))
            SceneManager.LoadScene(mainMenuSceneName);
    }
}