using UnityEngine;

public class MainMenuEpisodeProgressUI : MonoBehaviour
{
    [System.Serializable]
    private class EpisodeUI
    {
        public string episodeId;
        public GameObject completedLabel;
    }

    [SerializeField] private EpisodeUI[] episodes;

    [ContextMenu("Reset All Progress")]
    private void ResetAllProgress()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        Debug.Log("All progress deleted.");
    }
    
    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        foreach (var episode in episodes)
        {
            if (episode == null || episode.completedLabel == null)
                continue;

            episode.completedLabel.SetActive(
                EpisodeProgress.IsCompleted(episode.episodeId)
            );
        }
    }
}