using System;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(menuName = "Story/Story Node", fileName = "StoryNode")]
public class StoryNodeSM : ScriptableObject
{
    [Header("Video")]
    public VideoClip videoClip;

    [Header("Flow")]
    public bool isChoiceNode = false;
    public StoryNodeSM nextNode;

    [Header("Choice")]
    public ChoiceDataSM[] choices;
    public bool useTimer = false;
    public float choiceTime = 5f;
    public int defaultChoiceIndex = 0;

    [Header("When to show choices")]
    public float showChoicesBeforeEnd = 3f;
}

[Serializable]
public class ChoiceDataSM
{
    public string text;
    public StoryNodeSM nextNode;
}