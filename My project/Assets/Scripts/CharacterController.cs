using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour
{
    public UnityEngine.CharacterController cc;
    public Transform camera;
    [SerializeField] private float speed = 5f;

    void Awake()
    {
        if (camera == null) camera = Camera.main.transform;
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        
        Vector3 forward = camera.forward;
        Vector3 right = camera.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 move = (right * x + forward * z) * speed;

        cc.Move(move * Time.deltaTime);
    }
}
