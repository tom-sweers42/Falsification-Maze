using UnityEngine;
using TMPro;

public class ScoreSystem : MonoBehaviour
{
    public TMP_Text scoreText;
    private double score = 0.0;

    void Update()
    {
        if (score < 100)
        {
            score += 0.1;
            scoreText.text = "SCORE " + score.ToString("0");
        }
    }
}
