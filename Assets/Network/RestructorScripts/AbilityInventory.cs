using UnityEngine;
using Fusion;
using System;

public class AbilityInventory : NetworkBehaviour
{
    public Action<int> OnAbilitySet;
    public Action<int> OnAbilityUsed;

    [Networked] public int AbilityIndex { get; private set; } = -1;

    [Networked] public int UsesLeft { get; private set; } = 0;
    [Networked] public int MaxUses { get; private set; } = 0;

    [Networked] public float UseRechargeTime { get; private set; } = 0f;

    [Networked] public TickTimer rechageTimer { get; private set; } = TickTimer.None;


    public float RechargePercent => (rechageTimer.RemainingTime(Runner) ?? 0f) / UseRechargeTime;

    public void UseAbility()
    {
        if (AbilityIndex == -1)
            return;
        if (UsesLeft > 0)
        {
            UsesLeft--;
            
            rechageTimer = TickTimer.CreateFromSeconds(Runner, UseRechargeTime);
            OnAbilityUsed?.Invoke(AbilityIndex);
        }
    }

    public void SetAbility(int index, int maxUses, float rechargeTime)
    {
        AbilityIndex = index;
        MaxUses = maxUses;
        UsesLeft = maxUses;
        UseRechargeTime = rechargeTime;

        OnAbilitySet?.Invoke(index);
    }

    public override void FixedUpdateNetwork()
    {
        if (rechageTimer.Expired(Runner))
        {
            
            UsesLeft++;
            if (UsesLeft < MaxUses)
            {
                rechageTimer = TickTimer.CreateFromSeconds(Runner, UseRechargeTime);
            }
            else
            {
                rechageTimer = TickTimer.None;
            }
        }
    }






}
