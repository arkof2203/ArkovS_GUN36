using UnityEngine;

public class Pin : MonoBehaviour
{
    private bool isFallen = false;
    private float fallThreshold = 45f;
    private Vector3 initialPosition;
    public bool IsFallen => isFallen;


    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        if (!isFallen)
        {
            float tilt = Vector3.Angle(Vector3.up, transform.up);
            if (tilt > fallThreshold)
            {
                isFallen = true;
            }
        }
    }

    public void ResetPin()
    {
        isFallen = false;
        transform.position = initialPosition;
        transform.rotation = Quaternion.identity;

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Invoke(nameof(CountFallenPins), 3f);
        }
    }

    private void CountFallenPins()
    {
        GameManager.Instance.StartResetCountdown();
    }

}

