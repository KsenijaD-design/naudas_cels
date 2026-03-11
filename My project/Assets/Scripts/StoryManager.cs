using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;

public class StoryManager : MonoBehaviour
{
    [Header("Video")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private StoryNode startNode;

    [Header("Choice Canvases")]
    [SerializeField] private CanvasGroup twoChoiceCanvas;
    [SerializeField] private CanvasGroup threeChoiceCanvas;

    [Header("Two Choice Buttons")]
    [SerializeField] private Button[] twoChoiceButtons;

    [Header("Three Choice Buttons")]
    [SerializeField] private Button[] threeChoiceButtons;

    [Header("Timer UI")]
    [SerializeField] private CanvasGroup timerCanvas;
    [SerializeField] private TMP_Text timerText;

    [Header("Fade")]
    [SerializeField] private float fadeSpeed = 6f;

    private StoryNode currentNode;
    private Coroutine timerRoutine;

    private float twoChoiceTarget = 0f;
    private float threeChoiceTarget = 0f;
    private float timerTarget = 0f;

    void Start()
    {
        SetCanvasInstant(twoChoiceCanvas, 0f);
        SetCanvasInstant(threeChoiceCanvas, 0f);
        SetCanvasInstant(timerCanvas, 0f);

        PlayNode(startNode);
    }

    void Update()
    {
        FadeCanvas(twoChoiceCanvas, twoChoiceTarget);
        FadeCanvas(threeChoiceCanvas, threeChoiceTarget);
        FadeCanvas(timerCanvas, timerTarget);
    }

    void FadeCanvas(CanvasGroup cg, float target)
    {
        if (cg == null) return;

        cg.alpha = Mathf.MoveTowards(cg.alpha, target, fadeSpeed * Time.deltaTime);
        cg.interactable = cg.alpha > 0.99f;
        cg.blocksRaycasts = cg.alpha > 0.01f;
    }

    void SetCanvasInstant(CanvasGroup cg, float alpha)
    {
        if (cg == null) return;

        cg.alpha = alpha;
        cg.interactable = alpha > 0.99f;
        cg.blocksRaycasts = alpha > 0.01f;
    }

    public void PlayNode(StoryNode node)
    {
        if (node == null)
        {
            Debug.Log("Story ended.");
            return;
        }

        currentNode = node;

        twoChoiceTarget = 0f;
        threeChoiceTarget = 0f;
        timerTarget = 0f;

        if (timerRoutine != null)
        {
            StopCoroutine(timerRoutine);
            timerRoutine = null;
        }

        if (timerText) timerText.text = "";

        videoPlayer.Stop();
        videoPlayer.clip = node.videoClip;
        videoPlayer.loopPointReached -= OnVideoFinished;
        videoPlayer.loopPointReached += OnVideoFinished;
        videoPlayer.Play();
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        videoPlayer.loopPointReached -= OnVideoFinished;

        if (currentNode.isChoiceNode)
            ShowChoices();
        else
            PlayNode(currentNode.nextNode);
    }

    void ShowChoices()
    {
        int choiceCount = currentNode.choices.Length;

        if (choiceCount == 2)
        {
            twoChoiceTarget = 1f;
            threeChoiceTarget = 0f;

            for (int i = 0; i < twoChoiceButtons.Length; i++)
            {
                int index = i;

                TMP_Text txt = twoChoiceButtons[i].GetComponentInChildren<TMP_Text>();
                if (txt) txt.text = currentNode.choices[i].text;

                twoChoiceButtons[i].onClick.RemoveAllListeners();
                twoChoiceButtons[i].onClick.AddListener(() =>
                {
                    HideChoices();
                    PlayNode(currentNode.choices[index].nextNode);
                });
            }
        }
        else if (choiceCount == 3)
        {
            threeChoiceTarget = 1f;
            twoChoiceTarget = 0f;

            for (int i = 0; i < threeChoiceButtons.Length; i++)
            {
                int index = i;

                TMP_Text txt = threeChoiceButtons[i].GetComponentInChildren<TMP_Text>();
                if (txt) txt.text = currentNode.choices[i].text;

                threeChoiceButtons[i].onClick.RemoveAllListeners();
                threeChoiceButtons[i].onClick.AddListener(() =>
                {
                    HideChoices();
                    PlayNode(currentNode.choices[index].nextNode);
                });
            }
        }

        if (currentNode.useTimer)
        {
            timerTarget = 1f;
            timerRoutine = StartCoroutine(ChoiceTimer());
        }
        else
        {
            timerTarget = 0f;
            if (timerText) timerText.text = "";
        }
    }

    void HideChoices()
    {
        twoChoiceTarget = 0f;
        threeChoiceTarget = 0f;
        timerTarget = 0f;
    }

    IEnumerator ChoiceTimer()
    {
        float duration = currentNode.choiceTime;
        float t = duration;

        while (t > 0f)
        {
            t -= Time.deltaTime;

            if (timerText)
                timerText.text = Mathf.CeilToInt(t).ToString();

            yield return null;
        }

        HideChoices();

        int index = Mathf.Clamp(currentNode.defaultChoiceIndex, 0, currentNode.choices.Length - 1);
        PlayNode(currentNode.choices[index].nextNode);
    }
}