using System.Collections;
using UnityEngine;

public class ItemInspectController : MonoBehaviour
{
    [SerializeField] private Transform inspectPivot;
    [SerializeField] private InspectUI inspectUI;

    [Header("Crosshair Fade (CanvasGroup)")]
    [SerializeField] private CanvasGroup crosshairGroup;
    [SerializeField] private float crosshairFadeSpeed = 12f;

    [Header("Move")]
    [SerializeField] private float moveToInspectTime = 0.2f;
    [SerializeField] private float returnTime = 0.12f;

    [Header("Rotate")]
    [SerializeField] private float rotateSpeed = 220f;
    [SerializeField] private float resetTime = 0.15f;

    [SerializeField] private InspectBlur blur;
    
    private Interactable current;
    private Transform originalParent;
    private Vector3 originalPos;
    private Quaternion originalRot;

    private Coroutine co;

    private float crosshairTarget = 1f;

    public bool IsInspecting => current != null;

    void Awake()
    {
        if (crosshairGroup != null)
            crosshairTarget = crosshairGroup.alpha;
    }

    void Update()
    {
        if (crosshairGroup != null)
            crosshairGroup.alpha = Mathf.MoveTowards(crosshairGroup.alpha, crosshairTarget, crosshairFadeSpeed * Time.deltaTime);

        if (!IsInspecting) return;

        Transform cam = Camera.main.transform;
        
        if (Input.GetMouseButton(0))
        {
            float mx = Input.GetAxis("Mouse X");
            float my = Input.GetAxis("Mouse Y");

            current.transform.Rotate(cam.up, -mx * rotateSpeed * Time.deltaTime, Space.World);
            current.transform.Rotate(cam.right, my * rotateSpeed * Time.deltaTime, Space.World);
        }
        
        float horizontal = 0f;
        float vertical = 0f;
        
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            horizontal = 1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            horizontal = -1f;
        
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            vertical = -1f;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            vertical = 1f;

        if (horizontal != 0f)
        {
            current.transform.Rotate(cam.up, horizontal * rotateSpeed * Time.deltaTime, Space.World);
        }

        if (vertical != 0f)
        {
            current.transform.Rotate(cam.right, vertical * rotateSpeed * Time.deltaTime, Space.World);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (co != null) StopCoroutine(co);
            co = StartCoroutine(SmoothRotation(current.transform, Quaternion.identity, resetTime));
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
        {
            StopInspect();
        }
    }

    public void StartInspect(Interactable item)
    {
        if (blur) blur.SetBlur(true);
        
        if (item == null || IsInspecting) return;

        crosshairTarget = 0f;

        current = item;

        originalParent = item.transform.parent;
        originalPos = item.transform.position;
        originalRot = item.transform.rotation;

        if (inspectUI) inspectUI.SetText(item.itemName, item.itemDescription);

        GameManager.I.SetState(GameManager.GameState.Inspect);

        item.transform.SetParent(inspectPivot, true);

        if (co != null) StopCoroutine(co);
        co = StartCoroutine(Move(item.transform, inspectPivot.position, Quaternion.identity, moveToInspectTime));
    }

    public void StopInspect()
    {
        if (!IsInspecting) return;

        GameManager.I.SetState(GameManager.GameState.Gameplay);

        crosshairTarget = 1f;

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var cc = player.GetComponent<UnityEngine.CharacterController>();
            var moveScript = player.GetComponent<CharacterController>();

            if (cc != null) cc.enabled = false;
            if (moveScript != null) moveScript.enabled = false;
        }
        
        if (blur) blur.SetBlur(false);

        if (co != null) StopCoroutine(co);
        co = StartCoroutine(ReturnBackAndReEnableMovement());
    }

    IEnumerator ReturnBackAndReEnableMovement()
    {
        yield return Move(current.transform, originalPos, originalRot, returnTime);

        current.transform.SetParent(originalParent, true);
        current.transform.position = originalPos;
        current.transform.rotation = originalRot;

        current = null;

        yield return new WaitForSeconds(0.1f);

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var cc = player.GetComponent<UnityEngine.CharacterController>();
            var moveScript = player.GetComponent<CharacterController>();

            if (cc != null) cc.enabled = true;
            if (moveScript != null) moveScript.enabled = true;
        }
    }

    IEnumerator Move(Transform t, Vector3 targetPos, Quaternion targetRot, float time)
    {
        Vector3 startPos = t.position;
        Quaternion startRot = t.rotation;

        float elapsed = 0f;
        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            float k = Mathf.Clamp01(elapsed / time);
            k = k * k * (3f - 2f * k);

            t.position = Vector3.Lerp(startPos, targetPos, k);
            t.rotation = Quaternion.Slerp(startRot, targetRot, k);
            yield return null;
        }

        t.position = targetPos;
        t.rotation = targetRot;
    }

    IEnumerator SmoothRotation(Transform t, Quaternion targetLocal, float time)
    {
        Quaternion start = t.localRotation;
        float elapsed = 0f;

        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            float k = Mathf.Clamp01(elapsed / time);
            k = k * k * (3f - 2f * k);

            t.localRotation = Quaternion.Slerp(start, targetLocal, k);
            yield return null;
        }

        t.localRotation = targetLocal;
    }
}