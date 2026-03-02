using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager I;

    public enum GameState { Gameplay, Inspect, Pause }
    public GameState State { get; private set; } = GameState.Gameplay;

    [Header("Cameras")]
    [SerializeField] private GameObject firstPersonCameraObject;
    [SerializeField] private GameObject inspectCameraObject;

    [Header("Auto-disable scripts")]
    [SerializeField] private string[] disableScriptNames =
    {
        "CharacterController",
    };

    [Header("UI")]
    [SerializeField] private CanvasGroup hint;
    [SerializeField] private CanvasGroup inspectPanel;
    [SerializeField] private float uiFadeSpeed = 10f;

    private float hintTarget = 1f;
    private float inspectTarget = 0f;

    private Transform player;
    private UnityEngine.CharacterController cc;

    void Awake()
    {
        I = this;
        
        var playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO == null)
            playerGO = FindObjectOfType<UnityEngine.CharacterController>()?.gameObject;

        player = playerGO != null ? playerGO.transform : null;

        if (player != null)
            cc = player.GetComponent<UnityEngine.CharacterController>();

        ApplyState(GameState.Gameplay, instant: true);
    }

    void Update()
    {
        Fade(hint, hintTarget);
        Fade(inspectPanel, inspectTarget);

        // (пауза)
        // if (State == GameState.Gameplay && Input.GetKeyDown(KeyCode.Escape))
        //     ApplyState(GameState.Pause, instant: false);
    }

    void Fade(CanvasGroup g, float target)
    {
        if (g == null) return;
        g.alpha = Mathf.MoveTowards(g.alpha, target, uiFadeSpeed * Time.deltaTime);
        g.blocksRaycasts = g.alpha > 0.01f;
        g.interactable = g.alpha > 0.99f;
    }

    public void SetState(GameState newState) => ApplyState(newState, instant: false);

    void ApplyState(GameState newState, bool instant)
    {
        State = newState;
        bool inspecting = (State == GameState.Inspect);
        
        if (firstPersonCameraObject) firstPersonCameraObject.SetActive(!inspecting);
        if (inspectCameraObject) inspectCameraObject.SetActive(inspecting);
        
        Cursor.lockState = inspecting ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = inspecting;

        if (cc != null) cc.enabled = !inspecting;
        
        if (player != null)
        {
            var scripts = player.GetComponentsInChildren<MonoBehaviour>(true);
            foreach (var s in scripts)
            {
                if (s == null) continue;
                if (NameInList(s.GetType().Name, disableScriptNames))
                    s.enabled = !inspecting;
            }
        }

        hintTarget = inspecting ? 0f : 1f;
        inspectTarget = inspecting ? 1f : 0f;
        
        if (instant)
        {
            if (hint) hint.alpha = hintTarget;
            if (inspectPanel) inspectPanel.alpha = inspectTarget;
        }
    }

    bool NameInList(string name, string[] list)
    {
        for (int i = 0; i < list.Length; i++)
            if (list[i] == name) return true;
        return false;
    }
}