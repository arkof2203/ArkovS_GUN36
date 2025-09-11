using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [SerializeField] private Text scoreText;
    private List<int> rolls = new List<int>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddScore(int pins)
    {
        if (pins < 0 || pins > 10)
        {
            Debug.LogWarning($"Некорректное количество сбитых кегель: {pins}. Допустимый диапазон: 0-10.");
            return;
        }
        rolls.Add(pins);
        UpdateUI();
    }

    public void ResetScore()
    {
        rolls.Clear();
        UpdateUI();
    }

    private int CalculateScore()
    {
        int score = 0;
        int rollIndex = 0;

        for (int frame = 0; frame < 10; frame++)
        {
            if (rollIndex >= rolls.Count)
                break;

            if (IsStrike(rollIndex))
            {
                score += 10 + StrikeBonus(rollIndex);
                rollIndex += 1;
            }
            else if (IsSpare(rollIndex))
            {
                score += 10 + SpareBonus(rollIndex);
                rollIndex += 2;
            }
            else
            {
                score += SumOfFrame(rollIndex);
                rollIndex += 2;
            }
        }

        return score;
    }

    private bool IsStrike(int rollIndex)
    {
        return rollIndex < rolls.Count && rolls[rollIndex] == 10;
    }

    private bool IsSpare(int rollIndex)
    {
        return rollIndex + 1 < rolls.Count && rolls[rollIndex] + rolls[rollIndex + 1] == 10;
    }

    private int StrikeBonus(int rollIndex)
    {
        int bonus = 0;
        if (rollIndex + 1 < rolls.Count) bonus += rolls[rollIndex + 1];
        if (rollIndex + 2 < rolls.Count) bonus += rolls[rollIndex + 2];
        return bonus;
    }

    private int SpareBonus(int rollIndex)
    {
        if (rollIndex + 2 < rolls.Count) return rolls[rollIndex + 2];
        return 0;
    }

    private int SumOfFrame(int rollIndex)
    {
        int sum = 0;
        if (rollIndex < rolls.Count) sum += rolls[rollIndex];
        if (rollIndex + 1 < rolls.Count) sum += rolls[rollIndex + 1];
        return sum;
    }

    private void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + CalculateScore();
        }
        else
        {
            Debug.LogWarning("ScoreText не назначен в инспекторе!");
        }
    }
}


