using TMPro;
using UnityEngine;

public class CertificateProgressUI : MonoBehaviour
{
    [System.Serializable]
    private class CertificateTopic
    {
        public string episodeId;
        public TMP_Text topicText;
        public string unlockedText;
    }

    [Header("Certificate Topics")]
    [SerializeField] private CertificateTopic[] topics;

    [Header("Reward")]
    [SerializeField] private GameObject medalImage;

    private void Start()
    {
        RefreshCertificate();
    }

    public void RefreshCertificate()
    {
        bool allCompleted = true;

        foreach (var topic in topics)
        {
            if (topic == null || topic.topicText == null)
                continue;

            bool completed =
                EpisodeProgress.IsCompleted(topic.episodeId);

            topic.topicText.gameObject.SetActive(completed);

            if (completed)
            {
                topic.topicText.text =
                    topic.unlockedText;
            }
            else
            {
                allCompleted = false;
            }
        }

        if (medalImage != null)
            medalImage.SetActive(allCompleted);
    }
}