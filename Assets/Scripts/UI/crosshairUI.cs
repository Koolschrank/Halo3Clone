using UnityEngine;
using UnityEngine.UI;

public class crosshairUI : MonoBehaviour
{
    Color baseColor;

    [SerializeField] Color onTargetColor;
    [SerializeField] RawImage crosshairImage;


    public void ChangeSprite(Sprite sprite, Vector2 size)
    {
        if (crosshairImage != null)
        {
            if (sprite == null)
            {
                crosshairImage.enabled = false;
                return;
            }
            else 
            {
                crosshairImage.enabled = true;
            }

            crosshairImage.texture = sprite.texture;
            crosshairImage.rectTransform.localScale = size;
        }
    }

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
