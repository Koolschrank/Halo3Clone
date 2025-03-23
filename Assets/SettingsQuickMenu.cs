using UnityEngine;

public class SettingsQuickMenu : MonoBehaviour
{
    [SerializeField] RectTransform headUI;
    [SerializeField] RectTransform menuEnabledPostion;
    [SerializeField] RectTransform menuDisabledPostion;
    [SerializeField] float lerpSpeed = 10f;

    [SerializeField] float disableTime = 5f;
    float disableTimer = 0f;

    bool menuEnabled = false;


    [SerializeField] SensitivitySlider sensitivitySlider;

    private void Awake()
    {
        sensitivitySlider.OnUpdated += EnableMenu;

        DisableMenu();
        headUI.position = menuDisabledPostion.position;
    }

    public void EnableMenu()
    {
        menuEnabled = true;
        disableTimer = disableTime;
    }

    public void DisableMenu()
    {
        menuEnabled = false;
    }

    public void Update()
    {
        if (menuEnabled)
        {
            headUI.position = Vector3.Lerp(headUI.position, menuEnabledPostion.position, lerpSpeed * Time.deltaTime);
        }
        else
        {
            headUI.position = Vector3.Lerp(headUI.position, menuDisabledPostion.position, lerpSpeed * Time.deltaTime);
        }

        if (disableTimer > 0)
        {
            disableTimer -= Time.deltaTime;
        }
        else
        {
            DisableMenu();
        }
    }




}
