using UnityEngine;
using UnityEngine.UI;

public class ShildUI : MonoBehaviour
{
    [SerializeField] CharacterHealth health;
    [SerializeField] Image shildBar;

    Color defaultColor;
    [SerializeField] Color[] shildTeamColors;

    [SerializeField] Color alarmColor;
    [SerializeField] float alarmSpeed = 0.2f;
    float alarmTimer;
    bool inAlarm;
    bool alarmColorOn;

    private void Start()
    {
        if (defaultColor == null)
            defaultColor = shildBar.color;
    }

    public void SetTeamColor(int index)
    {
        defaultColor = shildTeamColors[index];
        shildBar.color = defaultColor;
    }


    public void SetUp(CharacterHealth health)
    {
        if (this.health != null)
        {
            this.health.OnShildChanged -= UpdateShild;
            this.health.OnShildDepleted -= ShildDepleted;
        }


        this.health = health;
        health.OnShildChanged += UpdateShild;
        health.OnShildDepleted += ShildDepleted;
        health.OnShildDisabled += DisableUI;
        health.OnShildEnabled += EnableUI;

        UpdateShild(health.ShildPercentage);


    }

    public void EnableUI()
    {
       
        gameObject.SetActive(true);
        UpdateShild(health.ShildPercentage);
    }

    public void DisableUI()
    {
        shildBar.fillAmount = 0;
        gameObject.SetActive(false);
    }

    public void UpdateShild(float shildValue)
    {
        if (inAlarm && shildValue != 0)
        {
            inAlarm = false;
            shildBar.color = defaultColor;
        }
        shildBar.fillAmount = shildValue;
    }

    public void ShildDepleted()
    {
        inAlarm = true;
        alarmTimer = 0;
        alarmColorOn = true;
        shildBar.color = alarmColor;
        shildBar.fillAmount = 1;
    }

    // update alarm
    private void Update()
    {
        if (inAlarm)
        {
            alarmTimer += Time.deltaTime;
            if (alarmTimer > alarmSpeed)
            {
                Color transparent = new Color(0, 0, 0, 0);
                alarmTimer = 0;
                alarmColorOn = !alarmColorOn;
                shildBar.color = alarmColorOn ? alarmColor : transparent;
            }
        }
    }








}
