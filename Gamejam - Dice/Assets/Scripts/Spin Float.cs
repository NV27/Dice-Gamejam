using UnityEngine;

public class SpinFloat : MonoBehaviour
{
    [Header("Spin Settings")]
    public Vector3 spinSpeed = new Vector3(0, 100, 0); // Degrees per second

    [Header("Float Settings")]
    public float floatAmplitude = 0.5f; // How high it floats
    public float floatSpeed = 1f; // How fast it floats

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Spin the object
        transform.Rotate(spinSpeed * Time.deltaTime);

        // Float the object up and down
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}