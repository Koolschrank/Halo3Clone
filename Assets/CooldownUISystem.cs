using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CooldownUISystem : MonoBehaviour
{
    [SerializeField] GameObject cooldownUIPrefab;
    [SerializeField] Transform[] cooldownUIParent;

    [SerializeField] bool hasPermanentObject = false;
    public CooldownUI permanentObject;
	List<CooldownUI> cooldownUIs = new List<CooldownUI>();

    AbilityInventory abilityInventory;

    public void Setup(AbilityInventory abilityInventory, PlayerMind mind)
    {
		if (hasPermanentObject)
		{
			permanentObject.gameObject.SetActive(false);
		}


		if (this.abilityInventory)
        {
            this.abilityInventory.OnAbilityAdded -= OnAbilityAdded;
            this.abilityInventory.OnAbilityRemoved -= OnAbilityRemoved;
            this.abilityInventory.OnAbilityIndexChanged -= UpdateSelection;


        }

        foreach (var item in cooldownUIs)
        {
            Destroy(item.gameObject);
        }
        cooldownUIs.Clear();
        this.abilityInventory = abilityInventory;


        

        abilityInventory.OnAbilityAdded += OnAbilityAdded;
        abilityInventory.OnAbilityRemoved += OnAbilityRemoved;
        abilityInventory.OnAbilityIndexChanged += UpdateSelection;

        UpdateSelection(abilityInventory.LastUsedAbility);
    }

    private void OnAbilityAdded(Ability ability, int index)
    {
        var cooldownUI = Instantiate(cooldownUIPrefab, cooldownUIParent[index]);
        var cooldownUIScript = cooldownUI.GetComponent<CooldownUI>();
        cooldownUIScript.Setup(ability);

        cooldownUIs.Add(cooldownUIScript);

        UpdatePositions();

        if (hasPermanentObject && index == 0 && !ability.abilityData.cannotUse)
        {
			permanentObject.gameObject.SetActive(true);

			permanentObject.Setup(ability);

		}



    }

    private void OnAbilityRemoved(Ability ability, int index)
    {
        if (index < cooldownUIs.Count)
        {
            Destroy(cooldownUIs[index].gameObject);
            cooldownUIs.RemoveAt(index);
            UpdatePositions();
        }
        if (hasPermanentObject && index == 0)
        {
            permanentObject.gameObject.SetActive(false);
		}
	}

    public void UpdatePositions()
    {
        if (cooldownUIs.Count == 0)
            return;
        else if (cooldownUIs.Count == 1)
        {
            cooldownUIs[0].transform.SetParent(cooldownUIParent[0]);
            var rectTransform1 = cooldownUIs[0].GetComponent<RectTransform>();
            rectTransform1.localPosition = Vector3.zero;
        }
        else if (cooldownUIs.Count == 2)
        {
            cooldownUIs[0].transform.SetParent(cooldownUIParent[0]);
            var rectTransform0 = cooldownUIs[0].GetComponent<RectTransform>();
            rectTransform0.localPosition = Vector3.zero;
            cooldownUIs[1].transform.SetParent(cooldownUIParent[1]);
            var rectTransform1 = cooldownUIs[1].GetComponent<RectTransform>();
            rectTransform1.localPosition = Vector3.zero;
        }
        else if (cooldownUIs.Count == 3)
        {
            cooldownUIs[0].transform.SetParent(cooldownUIParent[0]);
            var rectTransform0 = cooldownUIs[0].GetComponent<RectTransform>();
            rectTransform0.localPosition = Vector3.zero;
            cooldownUIs[1].transform.SetParent(cooldownUIParent[1]);
            var rectTransform1 = cooldownUIs[1].GetComponent<RectTransform>();
            rectTransform1.localPosition = Vector3.zero;
            cooldownUIs[2].transform.SetParent(cooldownUIParent[2]);
            var rectTransform2 = cooldownUIs[2].GetComponent<RectTransform>();
            rectTransform2.localPosition = Vector3.zero;
        }
    }

    public void UpdateSelection(int index)
    {
        for (int i = 0; i < cooldownUIs.Count; i++)
        {
            if (i == index)
            {
                cooldownUIs[i].SetSelected(true);
            }
            else
            {
                cooldownUIs[i].SetSelected(false);
            }
        }
    }





}
