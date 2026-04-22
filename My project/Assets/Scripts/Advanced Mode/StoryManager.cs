using UnityEngine;
using UnityEngine.Video;

public class StoryManagerAM : MonoBehaviour
{
    [Header("Video")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private StoryNodeAM startNode;

    [Header("Boards In Scene")]
    [SerializeField] private InvestigationBoardUI[] allBoards;

    [Header("Optional")]
    [SerializeField] private GameManagerAM gameManager;

    private StoryNodeAM currentNode;
    private InvestigationBoardUI activeBoard;

    private void Start()
    {
        HideAllBoardsInstant();
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
        Debug.Log("OnVideoFinished called for node: " + (currentNode != null ? currentNode.name : "NULL"));

        videoPlayer.loopPointReached -= OnVideoFinished;

        if (currentNode == null)
            return;

        if (currentNode.openBoardAfterVideo)
        {
            if (currentNode.boardIndex >= 0 && currentNode.boardIndex < allBoards.Length)
            {
                activeBoard = allBoards[currentNode.boardIndex];

                Debug.Log("Trying to show board index: " + currentNode.boardIndex);

                if (activeBoard != null)
                {
                    if (gameManager != null)
                        gameManager.SetBoardMode();

                    activeBoard.Show(null, OnBoardContinuePressed);
                    return;
                }
            }

            Debug.LogWarning("Invalid boardIndex or missing board");
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
            float rep = ReputationSystem.Instance != null ? ReputationSystem.Instance.CurrentReputation : 0f;

            if (rep >= currentNode.bestEndingThreshold && currentNode.bestEndingNode != null)
            {
                PlayNode(currentNode.bestEndingNode);
                return;
            }

            if (rep >= currentNode.goodEndingThreshold && currentNode.goodEndingNode != null)
            {
                PlayNode(currentNode.goodEndingNode);
                return;
            }

            if (currentNode.badEndingNode != null)
            {
                PlayNode(currentNode.badEndingNode);
                return;
            }

            Debug.LogWarning("StoryManager: badEndingNode is not assigned.");
            return;
        }

        PlayNode(currentNode.nextNode);
    }

    private void HideAllBoardsInstant()
    {
        if (allBoards == null) return;

        for (int i = 0; i < allBoards.Length; i++)
        {
            if (allBoards[i] != null)
                allBoards[i].HideInstant();
        }
    }
}