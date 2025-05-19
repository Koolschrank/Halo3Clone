using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;

public class AbilityInventory : MonoBehaviour
{
    public Action<Ability, int> OnAbilityAdded;
    public Action<Ability, int> OnAbilityRemoved;
    public Action<int> OnAbilityIndexChanged;
    public int LastUsedAbility { get; private set; } = 0;

    [SerializeField] int maxAbilities = 3;
    [SerializeField] List<Ability> abilities = new List<Ability>();
    [SerializeField] int currentAbilityIndex = 0;

    public List<Ability> Abilities => abilities;

    public void AddAbility(AbilityData abilityData)
    {
        var ability = new Ability(abilityData);
        abilities.Add(ability);
        OnAbilityAdded?.Invoke(ability, abilities.Count - 1);
        SelectNextAbilityWithChages();
    }

    public void RemoveAbility(Ability ability)
    {
        abilities.Remove(ability);
        OnAbilityRemoved?.Invoke(ability, abilities.Count);
    }

    public void Update()
    {
        UpdateAbilities(Time.deltaTime);
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
        cooldownTime = 0;
        charges = abilityData.maxCharges;
    }

    public void UpdateCooldown(float deltaTime)
    {
        Debug.Log("Updating cooldown for ability: " + abilityData.name);
        if (charges >= abilityData.maxCharges)
        {
            return;
        }

        if (cooldownTime > 0)
        {
            Debug.Log("Cooldown time: " + cooldownTime);
            cooldownTime -= deltaTime;
            if (cooldownTime < 0)
            {
                cooldownTime = 0;
            }
            OnCooldownChanged?.Invoke(1- cooldownTime/abilityData.cooldownTime);
        }

        if (cooldownTime <= 0)
        {
            Debug.Log("Cooldown finished for ability: " + abilityData.name);
            cooldownTime = 0;
            charges++;
            OnChargeGained?.Invoke(charges);


        }
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
