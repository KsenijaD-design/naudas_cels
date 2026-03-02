using UnityEngine;

public class InspectCameraYawFollow : MonoBehaviour
{
    [SerializeField] private Transform sourceCamera;
    [SerializeField] private float smooth = 0f;

    void Awake()
    {
        if (sourceCamera == null && Camera.main != null)
            sourceCamera = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (sourceCamera == null) return;

        float yaw = sourceCamera.eulerAngles.y;
        Quaternion target = Quaternion.Euler(0f, yaw, 0f);

        if (smooth <= 0f)
            transform.rotation = target;
        else
            transform.rotation = Quaternion.Slerp(transform.rotation, target, smooth * Time.deltaTime);
    }
}