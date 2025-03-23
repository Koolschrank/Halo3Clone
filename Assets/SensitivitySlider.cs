using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SensitivitySlider : MonoBehaviour
{
    public Action OnUpdated;

    [SerializeField] TextMeshProUGUI sensitivityTextValue;
    [SerializeField] Image bar;


    public void UpdateValues(float valueAsText, float valueAsPercentage)
    {
        // value 1 is 100%, value 0 is 0%, value 2 is 200%
        sensitivityTextValue.text = valueAsText.ToString("0.00")  + "%";

        bar.fillAmount = valueAsPercentage;
        OnUpdated?.Invoke();
    }


}
