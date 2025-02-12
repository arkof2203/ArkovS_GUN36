using UnityEngine;

public class RotateCube : MonoBehaviour
{
    public float rotationSpeed = 100f; // Скорость вращения

    void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}
