using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReputationSystem : MonoBehaviour
{
    public static ReputationSystem Instance { get; private set; }

    [Header("Value")]
    [SerializeField, Range(0f, 100f)] private float startReputation = 50f;
    [SerializeField] private float minReputation = 0f;
    [SerializeField] private float maxReputation = 100f;

    [Header("UI")]
    [SerializeField] private Slider reputationSlider;
    [SerializeField] private TMP_Text reputationText;

    public float CurrentReputation { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        CurrentReputation = Mathf.Clamp(startReputation, minReputation, maxReputation);
        RefreshUI();
    }

    public void AddPercent(float value)
    {
        CurrentReputation = Mathf.Clamp(CurrentReputation + value, minReputation, maxReputation);
        RefreshUI();
    }

    public void RemovePercent(float value)
    {
        AddPercent(-Mathf.Abs(value));
    }

    public void RefreshUI()
    {
        if (reputationSlider != null)
        {
            reputationSlider.minValue = minReputation;
            reputationSlider.maxValue = maxReputation;
            reputationSlider.value = CurrentReputation;
        }

        if (reputationText != null)
            reputationText.text = Mathf.RoundToInt(CurrentReputation) + "%";
    }
}
