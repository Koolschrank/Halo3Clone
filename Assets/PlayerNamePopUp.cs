using TMPro;
using UnityEngine;

public class PlayerNamePopUp : MonoBehaviour
{
    [SerializeField]  TextMeshProUGUI playerNameText;


    [SerializeField] bool showEnemyName = false;

    [SerializeField] Color allyColor = Color.green;
    [SerializeField] Color enemyColor = Color.red;

    public void SetUp(PlayerAim aim)
    {
        aim.OnHoverOnAlly += HoverOnAlly;
        aim.OnHoverOnNothing += HoverOff;
        aim.OnHoverOnEnemy += HoverOnEnemy;
    }


    public void HoverOnEnemy(string name)
    {
        if (showEnemyName)
        {
            playerNameText.text = name;
            playerNameText.color = enemyColor;
        }
        else
        {
            playerNameText.text = "";
        }
    }

    public void HoverOff()
    {
        Debug.Log("Hover on non: ");
        playerNameText.text = "";
    }

    public void HoverOnAlly(string ally)
    {
        
        playerNameText.text = ally;
        playerNameText.color = allyColor;
    }
}
