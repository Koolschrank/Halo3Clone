using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapUIObjective : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI numberText;
    [SerializeField] RectTransform rectTransform;

    public RectTransform RectTransform => rectTransform;

    public void ChangeColor(Color color)
    {
        image.color = color;
        numberText.color = color;
    }

    public void ChangeNumber(int number)
    {
        if (number < 0)
        {
            numberText.enabled = false;
            return;
        }
        numberText.text = number.ToString();
    }

    public void HideText()
    {
        numberText.enabled = false;
    }

    public void ShowText()
    {
        numberText.enabled = true;
    }
}
