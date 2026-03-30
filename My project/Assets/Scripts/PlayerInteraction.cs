using UnityEngine;
/*
public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private LayerMask interactableMask;
    [SerializeField] private ItemInspectController inspector;

    [Header("UI Hint")]
    [SerializeField] private CanvasGroup hintGroup;
    [SerializeField] private float fadeSpeed = 10f;

    private Interactable current;
    private float hintTarget = 0f;

    void Awake()
    {
        if (hintGroup) hintGroup.alpha = 0f;
        hintTarget = 0f;
    }

    void Update()
    {
        if (hintGroup)
            hintGroup.alpha = Mathf.MoveTowards(hintGroup.alpha, hintTarget, fadeSpeed * Time.deltaTime);

        if (GameManager.I.State != GameManager.GameState.Gameplay)
        {
            hintTarget = 0f;
            if (current != null) current.DisableOutline();
            return;
        }

        Check();

        if (current != null) current.EnableOutline();
        
        hintTarget = (current != null) ? 1f : 0f;

        if (Input.GetKeyDown(KeyCode.E) && current != null && inspector != null)
        {
            hintTarget = 0f;
            inspector.StartInspect(current);
        }
    }

    void Check()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        Interactable newI = null;
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactableMask, QueryTriggerInteraction.Ignore))
        {
            newI = hit.collider.GetComponent<Interactable>();
        }

        if (newI == current) return;

        if (current != null) current.DisableOutline();

        current = newI;

        if (current != null)
            current.EnableOutline();
    }
}*/