using UnityEngine;
using UnityEngine.Rendering;

public class InspectBlur : MonoBehaviour
{
    [SerializeField] private Volume volume;
    [SerializeField] private float fadeSpeed = 6f;

    float target;

    void Awake()
    {
        if (volume == null)
            volume = FindObjectOfType<Volume>(true);

        if (volume != null)
            target = volume.weight;
    }

    void Update()
    {
        if (volume == null) return;
        volume.weight = Mathf.MoveTowards(volume.weight, target, fadeSpeed * Time.deltaTime);
    }

    public void SetBlur(bool on)
    {
        target = on ? 1f : 0f;
    }
}