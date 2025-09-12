using UnityEngine;

public class BallThrow : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 startDragPos;
    private Vector3 endDragPos;
    private bool isThrown = false;

    [SerializeField] private float throwForce = 10f;
    [SerializeField] private float sideForce = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    void OnMouseDown()
    {
        if (isThrown) return;
        startDragPos = Input.mousePosition;
    }

    void OnMouseUp()
    {
        if (isThrown) return;
        endDragPos = Input.mousePosition; 
        ThrowBall();
    }

    void ThrowBall()
    {
        isThrown = true;
        rb.isKinematic = false;
        rb.useGravity = true;

        Vector3 dragVector = endDragPos - startDragPos;
        float forwardPower = dragVector.y * throwForce * 0.01f;
        float sidePower = dragVector.x * sideForce * 0.01f;

        Vector3 force = new Vector3(sidePower, 0, forwardPower);

        rb.AddForce(force, ForceMode.Impulse);
    }

    public void ResetThrow()
    {
        isThrown = false;
    }
}
