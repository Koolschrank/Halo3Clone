using TMPro;
using UnityEngine;

public class TeamWinUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

    private void Start()
    {
        text.enabled = false;
    }

    public void TeamWon(int teamIndex)
    {
        text.enabled = true;
        text.text = $"Team {teamIndex + 1} won!";
    }
}
