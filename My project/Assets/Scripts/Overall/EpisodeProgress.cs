using UnityEngine;

public static class EpisodeProgress
{
    private const string Prefix = "EpisodeCompleted_";

    public static void MarkCompleted(string episodeId)
    {
        if (string.IsNullOrEmpty(episodeId))
            return;

        PlayerPrefs.SetInt(Prefix + episodeId, 1);
        PlayerPrefs.Save();
    }

    public static bool IsCompleted(string episodeId)
    {
        if (string.IsNullOrEmpty(episodeId))
            return false;

        return PlayerPrefs.GetInt(Prefix + episodeId, 0) == 1;
    }

    public static void ResetProgress(string episodeId)
    {
        if (string.IsNullOrEmpty(episodeId))
            return;

        PlayerPrefs.DeleteKey(Prefix + episodeId);
        PlayerPrefs.Save();
    }
}