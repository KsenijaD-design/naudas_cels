using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceButtonUI : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text label;

    public void Setup(string text, Action onClick)
    {
        if (label != null)
            label.text = text;

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClick.Invoke());
        }
    }
}