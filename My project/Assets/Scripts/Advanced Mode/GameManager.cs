using UnityEngine;

public class GameManagerAM : MonoBehaviour
{
    public static GameManagerAM I;

    public enum GameState
    {
        Gameplay,
        Board,
        Pause
    }

    public GameState State { get; private set; } = GameState.Gameplay;

    [Header("Optional Player Root")]
    [SerializeField] private GameObject playerRoot;

    [Header("Scripts To Disable In Board Mode")]
    [SerializeField] private Behaviour[] gameplayScriptsToDisable;

    private CharacterController characterController;

    private void Awake()
    {
        I = this;

        if (playerRoot != null)
            characterController = playerRoot.GetComponent<CharacterController>();

        ApplyState(GameState.Gameplay);
    }

    private void LateUpdate()
    {
        if (State == GameState.Board)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void SetGameplayMode() => ApplyState(GameState.Gameplay);
    public void SetBoardMode() => ApplyState(GameState.Board);

    private void ApplyState(GameState newState)
    {
        State = newState;
        bool boardMode = State == GameState.Board;

        Cursor.lockState = boardMode ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = boardMode;

        if (characterController != null)
            characterController.enabled = !boardMode;

        if (gameplayScriptsToDisable != null)
        {
            for (int i = 0; i < gameplayScriptsToDisable.Length; i++)
            {
                if (gameplayScriptsToDisable[i] != null)
                    gameplayScriptsToDisable[i].enabled = !boardMode;
            }
        }
    }
}
