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
    public bool openBoardAfterVideo = false;
    public int boardIndex = -1;

    [Header("Ending By Reputation")]
    public bool useReputationEnding = false;
    public StoryNode badEndingNode;
    public StoryNode goodEndingNode;
    public StoryNode bestEndingNode;

    [Header("Thresholds")]
    [Range(0f, 100f)] public float goodEndingThreshold = 50f;
    [Range(0f, 100f)] public float bestEndingThreshold = 70f;
}