using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Skull : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public Image iconImage;



    public void SetSkull(Skull skull)
    {
        if (skull == null) return;
        nameText.text = skull.skullName;
        descriptionText.text = skull.skullDescription;
        //iconImage.sprite = skull.skullIcon;
        iconImage.color = skull.skullColor;
		gameObject.SetActive(true);
	}

    public void ClearSkull()
    {
        gameObject.SetActive(false);
	}
}
