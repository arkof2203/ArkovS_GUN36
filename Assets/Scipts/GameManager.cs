using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private Transform throwPoint;
    [SerializeField] private GameObject ball;
    [SerializeField] private List<Pin> pins;
    [SerializeField] private float resetDelay = 10f;

    private BallThrow ballThrowScript;
    private bool resetStarted = false;
    public List<Pin> Pins => pins;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        ballThrowScript = ball.GetComponent<BallThrow>();
    }

    public void StartResetCountdown()
    {
        if (!resetStarted)
        {
            resetStarted = true;

            int fallenPins = 0;
            foreach (var pin in pins)
            {
                if (pin.IsFallen) fallenPins++;
            }

            Debug.Log($"Adding {fallenPins} pins to score");

            ScoreManager.Instance.AddScore(fallenPins);

            Invoke(nameof(ResetPinsAndBall), resetDelay);
        }
    }




    private void ResetPinsAndBall()
    {
        // —брасываем кегли
        foreach (var pin in pins)
        {
            pin.ResetPin();
        }

        // —брасываем м€ч
        ball.transform.position = throwPoint.position;
        ball.transform.rotation = Quaternion.identity;

        Rigidbody rb = ball.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        rb.useGravity = false;

        ballThrowScript.ResetThrow();

        resetStarted = false;
    }

    public void CountFallenPinsAfterThrow()
    {
        if (!resetStarted)
        {
            resetStarted = true;

            int fallenPins = 0;
            foreach (var pin in pins)
            {
                if (pin.IsFallen) fallenPins++;
            }

            Debug.Log($"Adding {fallenPins} pins to score");

            ScoreManager.Instance.AddScore(fallenPins);

            Invoke(nameof(ResetPinsAndBall), resetDelay);
        }
    }

}

