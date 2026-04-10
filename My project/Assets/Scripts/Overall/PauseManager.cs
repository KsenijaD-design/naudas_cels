using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class PauseManager : MonoBehaviour
{
    public static PauseManager I;

    [SerializeField] private GameManagerSM gameManagerSM;
    
    [Header("UI")]
    [SerializeField] private GameObject pauseMenuUI;

    [Header("Optional")]
    [SerializeField] private VideoPlayer[] videoPlayersToPause;

    public bool IsPaused { get; private set; }
    public bool CanPause { get; private set; } = true;

    void Awake()
    {
        if (I == null) I = this;
        else Destroy(gameObject);

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
    }

    void Update()
    {
        if (gameManagerSM != null && gameManagerSM.IsInChoiceMode)
            return;
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!CanPause) return;

            if (IsPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void SetCanPause(bool value)
    {
        CanPause = value;
    }

    public void PauseGame()
    {
        if (IsPaused || !CanPause) return;

        IsPaused = true;

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);

        Time.timeScale = 0f;
        AudioListener.pause = true;

        foreach (var vp in videoPlayersToPause)
        {
            if (vp != null && vp.isPlaying)
                vp.Pause();
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        if (!IsPaused) return;

        IsPaused = false;

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        Time.timeScale = 1f;
        AudioListener.pause = false;

        foreach (var vp in videoPlayersToPause)
        {
            if (vp != null)
                vp.Play();
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void BackToMainMenu(string sceneName)
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        IsPaused = false;

        SceneManager.LoadScene(sceneName);
    }
}