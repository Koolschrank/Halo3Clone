using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;

public class AbilityInventory : MonoBehaviour
{
    public Action<Ability, int> OnAbilityAdded;
    public Action<Ability, int> OnAbilityRemoved;
    public Action<int> OnAbilityIndexChanged;
    public int LastUsedAbility { get; private set; } = 0;

    public int maxAbilities = 3;
    [SerializeField] List<Ability> abilities = new List<Ability>();
    [SerializeField] int currentAbilityIndex = 0;

    public List<Ability> Abilities => abilities;

    float cooldownMultiplier = 1f;
    [NonSerialized]
    public float abilityUseSpeedMultiplier = 1f;


    [SerializeField] PlayerBodyStatSheet playerBodyStatSheet;

    public bool alwaysSwitchToNewestAbility = false;
    public bool alwaysReplaceSecondAbility = false;

    private void Awake()
    {
        if (playerBodyStatSheet != null)
        {
            playerBodyStatSheet.OnStatSheetUpdated += OnStatChanged;
        }
    }

    private void OnStatChanged()
    {
        if (!playerBodyStatSheet.useStatSheet) return;
        cooldownMultiplier = playerBodyStatSheet.playerStatsSheetInstance.abilityCooldownMultiplier;
        abilityUseSpeedMultiplier = playerBodyStatSheet.playerStatsSheetInstance.abilityUseSpeedMultiplier;


    }


    public bool HasAbility()
    {
        return abilities.Count > 0;
	}
	public bool HasAbility(AbilityData abilityData)
    {
        foreach (var ability in abilities)
        {
            if (ability.abilityData == abilityData)
            {
                return true;
            }
        }
        return false;
    }

    public AbilityData GetAbility(int index)
        {
        if (index < 0 || index >= abilities.Count)
        {
            
            return null;
        }
        return abilities[index].abilityData;
    }

    public void RemoveAllAbilities()
    {
        foreach (var ability in abilities)
        {
            OnAbilityRemoved?.Invoke(ability, abilities.Count);
        }
        abilities.Clear();
        currentAbilityIndex = 0;
        LastUsedAbility = 0;
    }


    public void AddAbility(AbilityData abilityData)
    {
		if (alwaysReplaceSecondAbility && abilities.Count == 2)
		{
            RemoveAbility(1);
		}


		var ability = new Ability(abilityData);
        abilities.Add(ability);
        OnAbilityAdded?.Invoke(ability, abilities.Count - 1);
		if (alwaysSwitchToNewestAbility)
		{
			currentAbilityIndex = abilities.Count - 1;
			OnAbilityIndexChanged?.Invoke(currentAbilityIndex);
			LastUsedAbility = currentAbilityIndex;
		}
	}

    public void RemoveAbility(int index)
    {
        var ability = abilities[index];
		abilities.RemoveAt(index);

		OnAbilityRemoved?.Invoke(ability, index);
	}

    public void RemoveAbility(Ability ability)
    {
        abilities.Remove(ability);
        OnAbilityRemoved?.Invoke(ability, abilities.Count);
    }

    public void Update()
    {
        UpdateAbilities(Time.deltaTime * cooldownMultiplier);
    }

    public Ability GetLastAbility()
    {
        if (LastUsedAbility < 0 || LastUsedAbility >= abilities.Count)
        {
            return null;
        }
        return abilities[LastUsedAbility];
    }

    public void UpdateAbilities(float deltaTime)
    {
        foreach (var ability in abilities)
        {
            ability.UpdateCooldown(deltaTime);
        }
        if (!alwaysSwitchToNewestAbility)
            SelectNextAbilityWithChages();
    }

    public bool CanUseCurrentAbility()
    {
        if (currentAbilityIndex < 0 || currentAbilityIndex >= abilities.Count)
        {
            return false;
        }
        var currentAbility = abilities[currentAbilityIndex];
        return currentAbility.charges > 0;
    }

    public bool IsCurrentAbilityAGranade()
    {
        if (currentAbilityIndex < 0 || currentAbilityIndex >= abilities.Count)
        {
            return false;
        }
        var granadeAbility = abilities[currentAbilityIndex].abilityData as AbilityData_Granade;
        return granadeAbility.granadeStats != null;
    }

    public Ability GetFirstAbility()
    {
        if (abilities.Count == 0)
        {
            return null;
        }
        return abilities[0];
    }

    public Ability GetCurrentAbility()
    {
        if (currentAbilityIndex < 0 || currentAbilityIndex >= abilities.Count)
        {
            return null;
        }
        return abilities[currentAbilityIndex];
    }

    public void UseSelectedIndex()
    {
        if (currentAbilityIndex < 0 || currentAbilityIndex >= abilities.Count)
        {
            return;
        }
        var currentAbility = abilities[currentAbilityIndex];

        currentAbility.UseAbility();
        LastUsedAbility = currentAbilityIndex;
        SelectNextAbilityWithChages();


        if (currentAbility.ShouldRemoveWhenEmpty())
        {
            RemoveAbility(currentAbility);
			currentAbilityIndex = 0;
            OnAbilityIndexChanged?.Invoke(currentAbilityIndex);
			LastUsedAbility = currentAbilityIndex;

		}
       
    }

    public void UseAbility(int index)
    {
        if (index < 0 || index >= abilities.Count)
        {
            Debug.LogError("Invalid ability index");
            return;
        }
        abilities[index].UseAbility();

        LastUsedAbility = index;
    }

    public void SelectNextAbilityWithChages()
    {
        if (abilities.Count == 0)
        {
            return;
        }
        // check if current ability has charges
        if (abilities[currentAbilityIndex].charges > 0)
        {
            return;
        }
        if (abilities.Count == 1)
        {
            return;
        }


        int startIndex = currentAbilityIndex;
        do
        {
            currentAbilityIndex = (currentAbilityIndex + 1) % abilities.Count;
            if (abilities[currentAbilityIndex].charges > 0)
            {
                OnAbilityIndexChanged?.Invoke(currentAbilityIndex);
                return;
            }
        } while (currentAbilityIndex != startIndex);
        // If no ability with charges is found, reset to the first ability
        currentAbilityIndex = 0;
        OnAbilityIndexChanged?.Invoke(currentAbilityIndex);
    }






}


[Serializable]
public class Ability
{

    public Action<int> OnChargeGained;
    public Action<int> OnChargeLost;
    public Action<float> OnCooldownChanged;


    public AbilityData abilityData;
    public float cooldownTime;
    public int charges;

    public Ability(AbilityData abilityData)
    {
        this.abilityData = abilityData;
        cooldownTime = -1;
        charges = abilityData.maxCharges;
    }

    public void UpdateCooldown(float deltaTime)
    {
        
        if (charges >= abilityData.maxCharges)
        {
            return;
        }

        if (cooldownTime > 0)
        {
            cooldownTime -= deltaTime;
            if (cooldownTime < 0)
            {
                cooldownTime = 0;
            }
            OnCooldownChanged?.Invoke(1- cooldownTime/abilityData.cooldownTime);
        }

        if (cooldownTime <= 0 && charges < abilityData.maxCharges)
        {
            cooldownTime = 0;
            charges++;
            OnChargeGained?.Invoke(charges);


        }
    }

    public bool ShouldRemoveWhenEmpty()
    {
        return abilityData.removeWhenEmpty && charges <= 0;
	}

	public void UseAbility()
    {
        Debug.Log("Using ability: " + abilityData.name);
        if (charges > 0)
        {
            charges--;
            OnChargeLost?.Invoke(charges);
            cooldownTime = abilityData.cooldownTime;
            OnCooldownChanged?.Invoke(cooldownTime);

            
            
        }
    }
}
