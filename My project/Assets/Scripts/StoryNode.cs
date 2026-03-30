using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(menuName = "Story/Story Node", fileName = "StoryNode")]
public class StoryNode : ScriptableObject
{
    [Header("Video")]
    public VideoClip videoClip;

    [Header("Flow")]
    public StoryNode nextNode;

    [Header("Investigation Board")]
    [Tooltip("If true, after this video ends the board UI will open.")]
    public bool openBoardAfterVideo = false;
    public InvestigationBoardData boardData;

    [Header("Ending By Reputation")]
    [Tooltip("Enable only on the final checkpoint node if you want to branch by reputation.")]
    public bool useReputationEnding = false;
    public StoryNode badEndingNode;
    public StoryNode goodEndingNode;
    public StoryNode bestEndingNode;

    [Header("Thresholds")]
    [Range(0f, 100f)] public float goodEndingThreshold = 50f;
    [Range(0f, 100f)] public float bestEndingThreshold = 70f;
}
