using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(menuName = "Story/Story Node", fileName = "StoryNode")]
public class StoryNodeAM : ScriptableObject
{
    [Header("Video")]
    public VideoClip videoClip;

    [Header("Flow")]
    public StoryNodeAM nextNode;

    [Header("Investigation Board")]
    public bool openBoardAfterVideo = false;
    public int boardIndex = -1;

    [Header("Ending By Reputation")]
    public bool useReputationEnding = false;
    public StoryNodeAM badEndingNode;
    public StoryNodeAM goodEndingNode;
    public StoryNodeAM bestEndingNode;

    [Header("Thresholds")]
    [Range(0f, 100f)] public float goodEndingThreshold = 50f;
    [Range(0f, 100f)] public float bestEndingThreshold = 70f;
}