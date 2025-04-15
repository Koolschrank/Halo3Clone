using UnityEngine;
using UnityEngine.UI;

public class ShildUI : InterfaceItem
{
    [SerializeField] Image shildBar;

    Color defaultColor;
    [SerializeField] Color[] shildTeamColors;

    [SerializeField] Color alarmColor;
    [SerializeField] float alarmSpeed = 0.2f;
    float alarmTimer;
    bool inAlarm;
    bool alarmColorOn;


    protected override void Awake()
    {
        base.Awake();
        if (defaultColor == null)
            defaultColor = shildBar.color;
    }

    // subscribe to the player body
    protected override void Unsubscribe(PlayerBody body)
    {
        var health = body.Health as CharacterHealth;
        var team = body.PlayerTeam;

        health.OnShildChanged -= UpdateShild;
        health.OnShildDepleted -= ShildDepleted;
        health.OnShildDisabled -= DisableUI;
        health.OnShildEnabled -= EnableUI;
        team.OnTeamIndexChanged -= SetTeamColor;
    }

    protected override void Subscribe(PlayerBody body)
    {
        var health = body.Health as CharacterHealth;
        var team = body.PlayerTeam;
        health.OnShildChanged += UpdateShild;
        health.OnShildDepleted += ShildDepleted;
        health.OnShildDisabled += DisableUI;
        health.OnShildEnabled += EnableUI;
        team.OnTeamIndexChanged += SetTeamColor;

        if (health.MaxShild >0)
        {
            EnableUI();
        }
        else
        {
            DisableUI();
        }

         UpdateShild(health.ShildPercentage);


    }


    public void SetTeamColor(int index)
    {
        defaultColor = shildTeamColors[index];
        shildBar.color = defaultColor;
    }

    public void EnableUI()
    {
       
        gameObject.SetActive(true);
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
