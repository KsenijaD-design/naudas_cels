using UnityEngine;
using UnityEngine.Video;

public class VideoSkip : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private CanvasGroup fastForwardIcon;

    [Header("Episode Progress")]
    [SerializeField] private string episodeId = "episode_scams";
    [SerializeField] private bool requireEpisodeCompleted = true;

    [Header("Input")]
    [SerializeField] private KeyCode keyOne = KeyCode.RightArrow;
    [SerializeField] private KeyCode keyTwo = KeyCode.D;

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
        bool holding = Input.GetKey(keyOne) || Input.GetKey(keyTwo);
        bool shouldBoost = CanFastForward() && holding;

        if (videoPlayer != null)
            videoPlayer.playbackSpeed = shouldBoost ? boostedSpeed : normalSpeed;

        targetAlpha = shouldBoost ? 1f : 0f;
        FadeIcon();
    }

    private bool CanFastForward()
    {
        if (videoPlayer == null)
            return false;

        if (requireEpisodeCompleted && !EpisodeProgress.IsCompleted(episodeId))
            return false;

        return true;
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

    private void OnDestroy()
    {
        if (videoPlayer != null)
            videoPlayer.playbackSpeed = normalSpeed;
    }
}