using UnityEngine;
using TMPro;

public class InspectUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descText;

    public void SetText(string title, string desc)
    {
        if (titleText) titleText.text = title;
        if (descText) descText.text = desc;
    }
}