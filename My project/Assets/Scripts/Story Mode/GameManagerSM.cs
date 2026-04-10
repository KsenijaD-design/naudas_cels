using UnityEngine;

public class GameManagerSM : MonoBehaviour
{
    public enum State
    {
        Gameplay,
        Choice
    }

    public State Current { get; private set; }

    public bool IsInChoiceMode => Current == State.Choice;

    public void SetGameplayMode()
    {
        Current = State.Gameplay;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SetStoryChoiceMode()
    {
        Current = State.Choice;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}