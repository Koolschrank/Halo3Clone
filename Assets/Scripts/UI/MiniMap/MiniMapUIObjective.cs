using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapUIObjective : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI objectiveText;
    [SerializeField] RectTransform rectTransform;

    public RectTransform RectTransform => rectTransform;

    public void ChangeColor(Color color)
    {
        image.color = color;
        objectiveText.color = color;
    }

    public void ChangeText(string text)
    {
        
        objectiveText.text = text;
    }

    public void HideText()
    {
        objectiveText.enabled = false;
    }

    public void ShowText()
    {
        objectiveText.enabled = true;
    }

    
}
