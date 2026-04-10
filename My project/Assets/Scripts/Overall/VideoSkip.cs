using UnityEngine;
using UnityEngine.Video;

public class VideoSkipHoldSM : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private StoryManagerSM storyManager;
    [SerializeField] private CanvasGroup fastForwardIcon;

    [Header("Speed")]
    [SerializeField] private float normalSpeed = 1f;
    [SerializeField] private float boostedSpeed = 2f;

    [Header("Fade")]
    [SerializeField] private float fadeSpeed = 8f;

    private float targetAlpha;

    private void Awake()
    {
        if (fastForwardIcon != null)
        {
            fastForwardIcon.alpha = 0f;
            fastForwardIcon.interactable = false;
            fastForwardIcon.blocksRaycasts = false;
        }

        if (videoPlayer != null)
            videoPlayer.playbackSpeed = normalSpeed;
    }

    private void Update()
    {
        bool canFastForward = CanFastForward();
        bool holding = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);

        bool shouldBoost = canFastForward && holding;

        if (videoPlayer != null)
            videoPlayer.playbackSpeed = shouldBoost ? boostedSpeed : normalSpeed;

        targetAlpha = shouldBoost ? 1f : 0f;
        FadeIcon();
    }

    private bool CanFastForward()
    {
        if (videoPlayer == null || storyManager == null)
            return false;

        StoryNodeSM node = storyManager.CurrentNode;
        if (node == null)
            return false;

        return !node.isChoiceNode;
    }

    private void FadeIcon()
    {
        if (fastForwardIcon == null)
            return;

        fastForwardIcon.alpha = Mathf.MoveTowards(
            fastForwardIcon.alpha,
            targetAlpha,
            fadeSpeed * Time.deltaTime
        );

        fastForwardIcon.interactable = false;
        fastForwardIcon.blocksRaycasts = false;
    }

    private void OnDisable()
    {
        if (videoPlayer != null)
            videoPlayer.playbackSpeed = normalSpeed;

        if (fastForwardIcon != null)
            fastForwardIcon.alpha = 0f;
    }
}