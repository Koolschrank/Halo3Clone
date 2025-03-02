using TMPro;
using UnityEngine;

public class ScoreBarUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;


    public void SetScore(int score)
    {
        scoreText.text = score.ToString();
    }
}
