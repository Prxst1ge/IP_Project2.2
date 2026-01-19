using UnityEngine;
using UnityEngine.Rendering;

public class BlurVision : MonoBehaviour
{
    private Volume volume;

    void Start()
    {
        volume = GetComponent<Volume>();
        ActivateBlur();
    }
    [SerializeField]
    public void ActivateBlur()
    {
        // Smoothly turn the blur ON
        volume.weight = 1.0f;
    }
}