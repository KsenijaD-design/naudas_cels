using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChoiceButtonUISM : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text text;
    [SerializeField] private ChoiceCursorFollower cursor;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;

    private Action cachedAction;

    public void Setup(string label, Action action)
    {
        cachedAction = action;

        if (text != null)
            text.text = label;

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(HandleClick);
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlaySound(hoverSound);
    }

    private void HandleClick()
    {
        PlaySound(clickSound);
        cachedAction?.Invoke();
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource == null || clip == null)
            return;

        audioSource.PlayOneShot(clip);
    }
}