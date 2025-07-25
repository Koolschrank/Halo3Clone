using System;
using UnityEngine;

public class PlayerBuffs : MonoBehaviour
{
    [SerializeField] CharacterHealth health;


    public Action<Buff> OnEnterBuff;
	public Action OnExitBuff;
    public Action<float> OnUpdateBuff;

	Buff activeBuff;
    float startDuration;
    float duration;


    public void ApplyBuff(Buff buff)
    {
        health.RemoveArmor();

		if (activeBuff != null)
        {
            Debug.LogWarning("A buff is already active. Please remove the current buff before applying a new one.");
            return;
        }
        activeBuff = buff;
        startDuration = buff.duration;
        duration = buff.duration;
        // Additional logic to apply the buff effects can be added here
        buff.ApplyBuff(gameObject);
		OnEnterBuff?.Invoke(buff);


	}

    public void RemoveBuff()
    {
        if (activeBuff == null)
        {
            return;
        }
        activeBuff.RemoveBuff(gameObject);
		activeBuff = null;
        duration = 0f;


        OnExitBuff?.Invoke();
	}
    private void Update()
    {
        if (activeBuff != null)
        {
            duration -= Time.deltaTime;
            OnUpdateBuff?.Invoke((duration/startDuration));
			if (duration <= 0f)
            {
                RemoveBuff();
            }
        }
	}

}




