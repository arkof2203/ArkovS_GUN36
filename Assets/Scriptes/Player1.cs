using UnityEngine;

public class Player1 : MonoBehaviour
{
    public float moveSpeed = 5f;

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        Vector3 moveDir = new Vector3(moveX, moveY, 0).normalized;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }
}

