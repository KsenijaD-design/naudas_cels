using UnityEngine;

public class InspectPivotFollowXZ : MonoBehaviour
{
    [SerializeField] private Transform firstPersonCamera;
    [SerializeField] private float forwardOffset = 0.6f;
    [SerializeField] private Transform player;
    
    private float fixedY;

    void Start()
    {
        fixedY = transform.position.y;
    }

    void LateUpdate()
    {
        if (firstPersonCamera == null) return;
        
        Vector3 camPos = firstPersonCamera.position;
        
        Vector3 forwardFlat = new Vector3(firstPersonCamera.forward.x, 0f, firstPersonCamera.forward.z).normalized;

        Vector3 targetPos = camPos + forwardFlat * forwardOffset;

        targetPos.y = fixedY;

        transform.position = targetPos;
    }
}