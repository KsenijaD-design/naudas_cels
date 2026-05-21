using UnityEngine;
using UnityEngine.Video;

public class StoryManagerAM : MonoBehaviour
{
    [Header("Video")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private StoryNodeAM startNode;

    [Header("Boards In Scene")]
    [SerializeField] private InvestigationBoardUI[] allBoards;

    [Header("Newspapers In Scene")]
    [SerializeField] private NewspaperUI[] allNewspapers;

    [Header("Optional")]
    [SerializeField] private GameManagerAM gameManager;

    private StoryNodeAM currentNode;
    private InvestigationBoardUI activeBoard;

    private void Start()
    {
        HideAllBoardsInstant();
        HideAllNewspapersInstant();

        PlayNode(startNode);
    }

    public void PlayNode(StoryNodeAM node)
    {
        if (node == null)
        {
            Debug.Log("Story ended.");
            return;
        }

        currentNode = node;
        activeBoard = null;

        HideAllBoardsInstant();
        HideAllNewspapersInstant();

        if (videoPlayer == null)
        {
            Debug.LogError("StoryManager: VideoPlayer is missing.");
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
        videoPlayer.loopPointReached -= OnVideoFinished;

        if (currentNode == null)
            return;

        if (currentNode.showNewspaperBeforeBoard)
        {
            if (currentNode.newspaperIndex >= 0 &&
                currentNode.newspaperIndex < allNewspapers.Length)
            {
                NewspaperUI newspaper = allNewspapers[currentNode.newspaperIndex];

                if (newspaper != null)
                {
                    if (gameManager != null)
                        gameManager.SetBoardMode();

                    newspaper.Show(OnNewspaperContinuePressed);
                    return;
                }
            }
        }

        ShowBoardForCurrentNode();
    }

    private void OnNewspaperContinuePressed()
    {
        ShowBoardForCurrentNode();
    }

    private void ShowBoardForCurrentNode()
    {
        if (currentNode.openBoardAfterVideo)
        {
            if (currentNode.boardIndex >= 0 &&
                currentNode.boardIndex < allBoards.Length)
            {
                activeBoard = allBoards[currentNode.boardIndex];

                if (activeBoard != null)
                {
                    if (gameManager != null)
                        gameManager.SetBoardMode();

                    activeBoard.Show(null, OnBoardContinuePressed);
                    return;
                }
            }
        }

        ContinueFromNode();
    }

    private void OnBoardContinuePressed()
    {
        if (gameManager != null)
            gameManager.SetGameplayMode();

        ContinueFromNode();
    }

    private void ContinueFromNode()
    {
        if (currentNode == null)
            return;

        if (currentNode.useReputationEnding)
        {
            float rep = ReputationSystem.Instance != null
                ? ReputationSystem.Instance.CurrentReputation
                : 0f;

            if (rep >= currentNode.bestEndingThreshold &&
                currentNode.bestEndingNode != null)
            {
                PlayNode(currentNode.bestEndingNode);
                return;
            }

            if (rep >= currentNode.goodEndingThreshold &&
                currentNode.goodEndingNode != null)
            {
                PlayNode(currentNode.goodEndingNode);
                return;
            }

            if (currentNode.badEndingNode != null)
            {
                PlayNode(currentNode.badEndingNode);
                return;
            }

            return;
        }

        PlayNode(currentNode.nextNode);
    }

    private void HideAllBoardsInstant()
    {
        if (allBoards == null)
            return;

        for (int i = 0; i < allBoards.Length; i++)
        {
            if (allBoards[i] != null)
                allBoards[i].HideInstant();
        }
    }

    private void HideAllNewspapersInstant()
    {
        if (allNewspapers == null)
            return;

        for (int i = 0; i < allNewspapers.Length; i++)
        {
            if (allNewspapers[i] != null)
                allNewspapers[i].HideInstant();
        }
    }
}