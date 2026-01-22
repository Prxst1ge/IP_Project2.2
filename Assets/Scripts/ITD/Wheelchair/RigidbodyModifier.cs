using UnityEngine;

public class RigidbodyModifier : MonoBehaviour
{
    Rigidbody rb;

    [Header("Max Angular Velocity")]
    [SerializeField, Range(0, 100), Tooltip("Upward bounds of a Rigidbody's angular velocity. Unity default is 7.")]
    float maxAngularVelocity = 7f;

    [Header("Center of Mass")]
    [SerializeField, Tooltip("If unchecked, center of mass will be calculated automatically. Once a custom center is set, it will no longer be recomputed automatically.")]
    bool useCustomCenterOfMass = false;

    // This line requires the ConditionalHideAttribute script (see below)
    [SerializeField, ConditionalHide("useCustomCenterOfMass", true)]
    Vector3 centerOfMass;

    [SerializeField]
    bool visualizeCenterOfMass = false;
    GameObject visualization = null;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = maxAngularVelocity;

        if (useCustomCenterOfMass)
        {
            rb.centerOfMass = centerOfMass;
        }

        if (visualizeCenterOfMass)
        {
            CreateVisualization();
        }
    }

    void Update()
    {
        if (useCustomCenterOfMass)
        {
            rb.centerOfMass = centerOfMass;
        }
        else
        {
            rb.ResetCenterOfMass();
        }

        ManageVisualization();
    }

    // Helper method to keep Update clean
    void ManageVisualization()
    {
        if (!visualizeCenterOfMass && visualization)
        {
            Destroy(visualization);
            visualization = null;
        }

        if (visualizeCenterOfMass && !visualization)
        {
            CreateVisualization();
        }

        if (visualization)
        {
            visualization.transform.position = rb.worldCenterOfMass;
        }
    }

    void CreateVisualization()
    {
        visualization = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(visualization.GetComponent<Collider>()); // Better than disabling it
        visualization.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        visualization.GetComponent<MeshRenderer>().material.color = Color.magenta;
    }
}
