using UnityEngine;

public class MoveCubeY : MonoBehaviour
{
    public float moveDistance = 2f; // Расстояние перемещения
    public float moveSpeed = 2f;    // Скорость перемещения
    private Vector3 startPos;
    private bool movingUp = true;

    void Start()
    {
        startPos = transform.position; // Запоминаем начальное положение
    }

    void Update()
    {
        float newY = transform.position.y + (movingUp ? 1 : -1) * moveSpeed * Time.deltaTime;

        if (movingUp && newY >= startPos.y + moveDistance)
        {
            newY = startPos.y + moveDistance;
            movingUp = false;
        }
        else if (!movingUp && newY <= startPos.y)
        {
            newY = startPos.y;
            movingUp = true;
        }

        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}

