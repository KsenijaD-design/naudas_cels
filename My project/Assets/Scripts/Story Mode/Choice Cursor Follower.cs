using UnityEngine;
using UnityEngine.EventSystems;

public class ChoiceCursorFollower : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Refs")]
    [SerializeField] private RectTransform center;
    [SerializeField] private RectTransform arrowPivot;

    [Header("Settings")]
    [SerializeField] private float smoothSpeed = 12f;
    [SerializeField] private float angleOffset = 0f;

    private bool locked;

    private float currentAngle;
    private float targetAngle;

    private void Awake()
    {
        ResetArrow();
    }

    private void Update()
    {
        if (center == null || arrowPivot == null)
            return;
        
        if (!locked)
        {
            Vector2 centerScreen = RectTransformUtility.WorldToScreenPoint(null, center.position);
            Vector2 dir = (Vector2)Input.mousePosition - centerScreen;

            if (dir.sqrMagnitude > 0.001f)
            {
                targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + angleOffset;
            }
        }
        
        currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * smoothSpeed);

        arrowPivot.localRotation = Quaternion.Euler(0f, 0f, currentAngle);
    }

    public void ResetArrow()
    {
        locked = false;

        currentAngle = angleOffset;
        targetAngle = angleOffset;

        if (arrowPivot != null)
            arrowPivot.localRotation = Quaternion.Euler(0f, 0f, currentAngle);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        locked = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        locked = false;
    }
}