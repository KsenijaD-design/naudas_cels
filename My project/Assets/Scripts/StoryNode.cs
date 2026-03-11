using System;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(menuName = "Story/Story Node", fileName = "StoryNode")]
public class StoryNode : ScriptableObject
{
    [Header("Video")]
    public VideoClip videoClip;

    [Header("Flow")]
    public bool isChoiceNode = false;
    public StoryNode nextNode;

    [Header("Choice")]
    public ChoiceData[] choices;
    public bool useTimer = false;
    public float choiceTime = 5f;
    public int defaultChoiceIndex = 0;
}

[Serializable]
public class ChoiceData
{
    public string text;
    public StoryNode nextNode;
}