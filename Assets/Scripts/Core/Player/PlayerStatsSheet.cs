using System;
using UnityEngine;


[CreateAssetMenu(fileName = "PlayerStatsSheet", menuName = "ScriptableObjects/PlayerStatsSheet", order = 1)]
public class PlayerStatsSheet : ScriptableObject
{
    public Action<PassiveEffectType, bool> OnPassiveEffectChanged;
    public Action<StatType, float> OnStatChanged;


    public float maxHealth = 100;
    public float healthRegenPerSecond = 1f;
    public float healthRegenDelay = 5f;

    public float maxShild = 100;
    public float shieldRegenPerSecond = 1f;
    public float shieldRegenDelay = 5f;

    public float weaponDamageMultiplier = 1;
    public float abilityDamageMultiplier = 1;
    public float meleeDamageMultiplier = 1;

    public float fireRateMultiplier = 1f;
    public float reloadSpeedMultiplier = 1f;
    public float magazineSizeMultiplier = 1f;
    public float weaponSwitchSpeedMultiplier = 1f;
    public float abilityUseSpeedMultiplier = 1f;

    public float abilityCooldownMultiplier = 1f;
    public float movementSpeedMultiplier = 1f;
    public float ammoRegenerationMultiplierChance = 0f;
    public float reviveSpeedMultiplier = 1f;
    public float meleeSpeedMultiplier = 1f;

    public int abilitySlots = 1;



    [Header("Unique Abilities")]
    public bool dualWielding = false;
    public float healOnMeleeKillAmount = 20f;
    public float noShildDamageMultiplier = 1f;
    public float noShildMovementSpeedMultiplier = 1f;
    public float noShildFireRateMultiplier = 1f;
    public float noShildReloadMultiplier = 1f;
    public float noShildMeleeSpeedMultiplier = 1f;
    public float noShildSwitchWeaponSpeedMultiplier = 1f;
    public float crouchBuff_firerateMultiplier;
    public float crouchBuff_reloadMultiplier;


    public bool instantRevive = false;
    public bool expesiveBullets = false;

    public void SetPassiveEffect(PassiveEffectType effect, bool value)
        {
        switch (effect)
        {
            case PassiveEffectType.dualWielding:
                dualWielding = value;
                break;
            case PassiveEffectType.instantRevive:
                instantRevive = value;
                break;
            case PassiveEffectType.expensiveBullets:
                expesiveBullets = value;
                break;
            default:
                Debug.LogWarning("Unknown passive effect type: " + effect);
                break;
        }

        OnPassiveEffectChanged?.Invoke(effect, value);
    }

    public void AddStat(StatType type, float value)
        {
        switch (type)
        {
            case StatType.Health:
                maxHealth += value;
                break;
            case StatType.HealthRegen:
                healthRegenPerSecond += value;
                break;
            case StatType.HealthRegenDelay:
                healthRegenDelay += value;
                break;
            case StatType.Shield:
                maxShild += value;
                break;
            case StatType.ShieldRegen:
                shieldRegenPerSecond += value;
                break;
            case StatType.ShieldRegenDelay:
                shieldRegenDelay += value;
                break;
            case StatType.WeaponDamage:
                weaponDamageMultiplier += value;
                break;
            case StatType.AbilityDamage:
                abilityDamageMultiplier += value;
                break;
            case StatType.MeleeDamage:
                meleeDamageMultiplier += value;
                break;
            case StatType.FireRate:
                fireRateMultiplier += value;
                break;
            case StatType.ReloadSpeed:
                reloadSpeedMultiplier += value;
                break;
            case StatType.MagazineSize:
                magazineSizeMultiplier += value;
                break;
            case StatType.WeaponSwitchSpeed:
                weaponSwitchSpeedMultiplier += value;
                break;
            case StatType.AbilityUseSpeed:
                abilityUseSpeedMultiplier += value;
                break;
            case StatType.AbilityCooldown:
                abilityCooldownMultiplier += value;
                break;
            case StatType.MovementSpeed:
                movementSpeedMultiplier += value;
                break;
            case StatType.AmmoRegenerationChance:
                ammoRegenerationMultiplierChance += value; 
                break;
            case StatType.ReviveSpeed:
                reviveSpeedMultiplier += value; 
                break;
            case StatType.MeleeSpeed:
                meleeSpeedMultiplier += value; 
                break;


            case StatType.healOnMeleeKill:
                healOnMeleeKillAmount += value;
                break;
            case StatType.noShieldDamageMultiplier:
                noShildDamageMultiplier += value;
                break;
            case StatType.noShieldMovementSpeedMultiplier:
                noShildMovementSpeedMultiplier += value;
                break;
            case StatType.noShieldFireRateMultiplier:
                noShildFireRateMultiplier += value;
                break;
            case StatType.noShieldReloadMultiplier:
                noShildReloadMultiplier += value;
                break;
            case StatType.noShieldMeleeSpeedMultiplier:
                noShildMeleeSpeedMultiplier += value;
                break;
            case StatType.noShieldSwitchWeaponSpeedMultiplier:
                noShildSwitchWeaponSpeedMultiplier += value;
                break;
            case StatType.crouchBuff_firerateMultiplier:
                crouchBuff_firerateMultiplier += value;
                break;
            case StatType.crouchBuff_reloadMultiplier:
                crouchBuff_reloadMultiplier += value;
                break;



            default:
                Debug.LogWarning("Unknown stat type: " + type);
                break;
        }

        // Trigger the stat changed event if it exists
        OnStatChanged?.Invoke(type, value);
    }


    public PlayerStatsSheet(PlayerStatsSheet statsToCopy)
    {
        maxHealth = statsToCopy.maxHealth;
        healthRegenPerSecond = statsToCopy.healthRegenPerSecond;
        healthRegenDelay = statsToCopy.healthRegenDelay;
        maxShild = statsToCopy.maxShild;
        shieldRegenPerSecond = statsToCopy.shieldRegenPerSecond;
        shieldRegenDelay = statsToCopy.shieldRegenDelay;
        weaponDamageMultiplier = statsToCopy.weaponDamageMultiplier;
        abilityDamageMultiplier = statsToCopy.abilityDamageMultiplier;
        meleeDamageMultiplier = statsToCopy.meleeDamageMultiplier;
        fireRateMultiplier = statsToCopy.fireRateMultiplier;
        reloadSpeedMultiplier = statsToCopy.reloadSpeedMultiplier;
        magazineSizeMultiplier = statsToCopy.magazineSizeMultiplier;
        weaponSwitchSpeedMultiplier = statsToCopy.weaponSwitchSpeedMultiplier;
        abilityUseSpeedMultiplier = statsToCopy.abilityUseSpeedMultiplier;
        abilityCooldownMultiplier = statsToCopy.abilityCooldownMultiplier;
        movementSpeedMultiplier = statsToCopy.movementSpeedMultiplier;
        ammoRegenerationMultiplierChance = statsToCopy.ammoRegenerationMultiplierChance;
        reviveSpeedMultiplier = statsToCopy.reviveSpeedMultiplier;
        meleeSpeedMultiplier = statsToCopy.meleeSpeedMultiplier;
        abilitySlots = statsToCopy.abilitySlots;
        dualWielding = statsToCopy.dualWielding;
        healOnMeleeKillAmount = statsToCopy.healOnMeleeKillAmount;
        noShildDamageMultiplier = statsToCopy.noShildDamageMultiplier;
        noShildMovementSpeedMultiplier = statsToCopy.noShildMovementSpeedMultiplier;
        noShildFireRateMultiplier = statsToCopy.noShildFireRateMultiplier;
        noShildReloadMultiplier = statsToCopy.noShildReloadMultiplier;
        noShildMeleeSpeedMultiplier = statsToCopy.noShildMeleeSpeedMultiplier;
        noShildSwitchWeaponSpeedMultiplier = statsToCopy.noShildSwitchWeaponSpeedMultiplier;
        crouchBuff_firerateMultiplier = statsToCopy.crouchBuff_firerateMultiplier;
        crouchBuff_reloadMultiplier = statsToCopy.crouchBuff_reloadMultiplier;
        instantRevive = statsToCopy.instantRevive;
        expesiveBullets = statsToCopy.expesiveBullets;
    }

}


public enum PassiveEffectType
{
    dualWielding = 0,
    instantRevive = 1,
    expensiveBullets = 2,
}

public enum StatType
    {
    Health,
    HealthRegen,
    HealthRegenDelay,
    Shield,
    ShieldRegen,
    ShieldRegenDelay,
    WeaponDamage,
    AbilityDamage,
    MeleeDamage,
    FireRate,
    ReloadSpeed,
    MagazineSize,
    WeaponSwitchSpeed,
    AbilityUseSpeed,
    AbilityCooldown,
    MovementSpeed,
    AmmoRegenerationChance,
    ReviveSpeed,
    MeleeSpeed,

    healOnMeleeKill = 50,
    noShieldDamageMultiplier = 51,
    noShieldMovementSpeedMultiplier = 52,
    noShieldFireRateMultiplier = 53,
    noShieldReloadMultiplier = 54,
    noShieldMeleeSpeedMultiplier = 55,
    noShieldSwitchWeaponSpeedMultiplier = 56,
    crouchBuff_firerateMultiplier = 57,
    crouchBuff_reloadMultiplier = 58,

}
