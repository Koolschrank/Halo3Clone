using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBarUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] Image scoreBar;
    [SerializeField] float minFill = 0.2f;

    int maxScore = 10;
    public void SetMaxScore(int maxScore)
    {
        this.maxScore = maxScore;
    }

    private void Start()
    {
        SetScore(0);
    }

    public void SetScore(int score)
    {
        scoreText.text = score.ToString();
        SetFill(score);
    }

    public void SetFill(int score)
    {
        Debug.Log(maxScore);
        float fill = ((float)score / maxScore * (1 - minFill))  + minFill;
        Debug.Log(fill);
        scoreBar.fillAmount = fill;

    }
}
