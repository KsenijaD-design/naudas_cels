using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    private Outline outline;

    [Header("Inspect Text")]
    public string itemName;
    [TextArea(2, 6)] public string itemDescription;

    public UnityEvent onInteraction;

    void Start()
    {
        outline = GetComponent<Outline>();
        outline.OutlineMode = Outline.Mode.SilhouetteOnly;
        outline.OutlineWidth = 0f;
    }

    public void DisableOutline()
    {
        outline.OutlineWidth = 0f;
        outline.OutlineMode = Outline.Mode.SilhouetteOnly;
    }

    public void EnableOutline()
    {
        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineWidth = 5f;
    }
}