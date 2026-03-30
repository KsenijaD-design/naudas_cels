using UnityEngine;
using UnityEngine.Video;

public class StoryManager : MonoBehaviour
{
    [Header("Video")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private StoryNode startNode;

    [Header("Board UI")]
    [SerializeField] private InvestigationBoardUI boardUI;

    [Header("Optional")]
    [SerializeField] private GameManager gameManager;

    private StoryNode currentNode;

    private void Start()
    {
        if (boardUI != null)
            boardUI.HideInstant();

        PlayNode(startNode);
    }

    public void PlayNode(StoryNode node)
    {
        if (node == null)
        {
            Debug.Log("Story ended.");
            return;
        }

        currentNode = node;

        if (boardUI != null)
            boardUI.HideInstant();

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

        if (currentNode.openBoardAfterVideo && currentNode.boardData != null && boardUI != null)
        {
            videoPlayer.Stop();

            if (gameManager != null)
                gameManager.SetBoardMode();

            boardUI.Show(currentNode.boardData, OnBoardContinuePressed);
            return;
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
}
