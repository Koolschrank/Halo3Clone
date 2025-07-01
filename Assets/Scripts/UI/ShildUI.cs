using UnityEngine;
using UnityEngine.UI;

public class ShildUI : MonoBehaviour
{
    [SerializeField]
	ShildEffectUI shildEffectUI;
	[SerializeField] PlayerCamera playerCam;
	[SerializeField] CharacterHealth health;
    [SerializeField] Image shildBar;
	[SerializeField] Image shildBar_damage;

	[SerializeField] Image shildBar_damageColor;
	[SerializeField] Image shildBarColor;

	Color defaultColor;
    [SerializeField] Color[] shildTeamColors;

    [SerializeField] Color alarmColor;
    [SerializeField] float alarmSpeed = 0.2f;
    float alarmTimer;
    bool inAlarm;
    bool alarmColorOn;


    [SerializeField] RectTransform shildBarRect;
    [SerializeField] RectTransform shildBarRectBackground;

    [SerializeField] float shortBarWidth = 500f;
    [SerializeField] float longBarWidth = 1000f;
    [SerializeField] float damageBarTime = 0.2f;
    [SerializeField] float shildRegenWidth = 0.1f; // width of the shild regen bar in percentage
	[SerializeField] AnimationCurve damageBarVisibilityCurve;
    float damageBarTimer = 0f;


	public void SetWidth(bool lenght)
    {
        return;
        if (lenght)
        {
            shildBarRect.sizeDelta = new Vector2(longBarWidth -20, shildBarRect.sizeDelta.y);
            shildBarRectBackground.sizeDelta = new Vector2(longBarWidth, shildBarRectBackground.sizeDelta.y);

        }
        else
        {
            shildBarRect.sizeDelta = new Vector2(shortBarWidth - 20, shildBarRect.sizeDelta.y);
            shildBarRectBackground.sizeDelta = new Vector2(shortBarWidth, shildBarRectBackground.sizeDelta.y);
        }

    }

    private void Start()
    {
        if (defaultColor == null)
            defaultColor = shildBarColor.color;
    }

    public void SetTeamColor(int index)
    {
        defaultColor = shildTeamColors[index];
		shildBarColor.color = defaultColor;
    }


    public void SetUp(CharacterHealth health)
    {
        if (this.health != null)
        {
            this.health.OnShildChanged -= UpdateShild;
            this.health.OnShildDepleted -= ShildDepleted;
            this.health.OnShildDisabled -= DisableUI;
        }


        this.health = health;
        health.OnShildChanged += UpdateShild;
        health.OnShildDepleted += ShildDepleted;
        health.OnShildDisabled += DisableUI;
        health.OnShildEnabled += EnableUI;
        health.OnMaxShildChanged += UpdateMaxShildUI;

        UpdateShild(health.ShildPercentage);

        SetWidth( health.MaxShild > 55f);


    }

    public void UpdateMaxShildUI(float maxShildValue)
    {
        if (maxShildValue > 55f)
        {
            SetWidth(true);
        }
        else
        {
            SetWidth(false);
        }
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

    float lastShildValue = 0f;
	public void UpdateShild(float shildValue)
    {
        if (lastShildValue> shildValue)
        {
            shildEffectUI.TriggerEffect();
            if (shildValue <=0)
                shildEffectUI.Stop(); // stop effect if shild is depleted


			playerCam.EnterShildBloom(); // trigger bloom effect on shild change

            if (damageBarTimer<= 0f)
                shildBar_damage.fillAmount = lastShildValue;
            
            damageBarTimer = damageBarTime;
		}
        else if (lastShildValue < shildValue)
        {
		    shildBar_damage.fillAmount = shildValue + shildRegenWidth;
			damageBarTimer = damageBarTime;
		}
		

			if (inAlarm && shildValue != 0)
        {
            inAlarm = false;
            shildBarColor.color = defaultColor;
			shildBar.gameObject.SetActive(true);
		}
        shildBar.fillAmount = shildValue;


		
        

        lastShildValue = shildValue;
	}



    public void ShildDepleted()
    {
        inAlarm = true;
        alarmTimer = 0;
        alarmColorOn = true;
		shildBarColor.color = alarmColor;
        shildBar.fillAmount = 1;

        shildBar_damage.fillAmount = 0;
        damageBarTimer = 0f;
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
                if (alarmColorOn)
                {
                    shildBarColor.color = alarmColor;
                    shildBar.gameObject.SetActive(true);

				}
                else
                {
					shildBarColor.color = new Color(0, 0, 0, 0);
                    shildBar.gameObject.SetActive(false);
				}

            }
        }
        else
        {
            if (damageBarTimer > 0)
            {
                damageBarTimer -= Time.deltaTime;
                float visibility = damageBarVisibilityCurve.Evaluate(1f - (damageBarTimer / damageBarTime));

                var c = shildBar_damageColor.color;
				shildBar_damageColor.color = new Color(c.r, c.g, c.b, visibility);

            }
            else             {
                shildBar_damageColor.color = new Color(shildBar_damageColor.color.r, shildBar_damageColor.color.g, shildBar_damageColor.color.b, 0f);
                shildBar_damage.fillAmount = 0f;
			}
		}
            
    }








}
