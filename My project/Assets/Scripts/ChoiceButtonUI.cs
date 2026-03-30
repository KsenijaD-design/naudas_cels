using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Left for compatibility if this script is already used somewhere in the scene.
// It is no longer the main story mechanic.
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
            button.onClick.AddListener(() => onClick?.Invoke());
        }
    }
}
