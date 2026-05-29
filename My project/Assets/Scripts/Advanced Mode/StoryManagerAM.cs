using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

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
    [SerializeField] private string mainMenuSceneName = "Main Menu";

    [Header("Episode Progress")]
    [SerializeField] private string episodeId = "episode_1A";
    
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
            ReturnToMainMenu();
            return;
        }

        currentNode = node;
        activeBoard = null;

        HideAllBoardsInstant();
        HideAllNewspapersInstant();

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

        if (currentNode.showNewspaperBeforeBoard)
        {
            if (allNewspapers != null &&
                currentNode.newspaperIndex >= 0 &&
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
        if (currentNode == null)
        {
            ReturnToMainMenu();
            return;
        }

        if (currentNode.openBoardAfterVideo)
        {
            if (allBoards != null &&
                currentNode.boardIndex >= 0 &&
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
        {
            ReturnToMainMenu();
            return;
        }

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

            ReturnToMainMenu();
            return;
        }

        if (currentNode.nextNode != null)
        {
            PlayNode(currentNode.nextNode);
            return;
        }

        ReturnToMainMenu();
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