using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("”правление")]
    [SerializeField] private float moveSpeed = 5f; 
    private Rigidbody2D rb;

    private Vector2 movement; 
    private Vector2 mousePos;
    private Camera cam;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 0f;
        rb.rotation = angle;
    }
}
