using UnityEngine;
using UnityEngine.UI;

public class crosshairUI : MonoBehaviour
{
    Color baseColor;

    [SerializeField] Color onTargetColor;
    [SerializeField] RawImage crosshairImage;

    //start
    private void Start()
    {
        baseColor = crosshairImage.color;
    }

    // target acquired
    public void OnTargetAcquired(Transform target)
    {
        crosshairImage.color = onTargetColor;
    }

    // target lost
    public void OnTargetLost(Transform target)
    {
        crosshairImage.color = baseColor;
    }
}
