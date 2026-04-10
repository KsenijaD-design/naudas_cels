using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceButtonUISM : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text text;
    [SerializeField] private ChoiceCursorFollower cursor;

    public void Setup(string label, Action action)
    {
        if (text != null)
            text.text = label;

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => action?.Invoke());
        }

        if (cursor != null)
            cursor.ResetArrow();
    }

    public void SetActive(bool state)
    {
        gameObject.SetActive(state);

        if (state && cursor != null)
            cursor.ResetArrow();
    }
}