using UnityEngine;

public class UI_SkullList : MonoBehaviour
{
    public UI_Skull[] ui_skulls;
	int index = 0;


	private void Awake()
	{
		var skullManager = SkullManager.instance;

		skullManager.OnClearSkulls += ClearSkulls;
		skullManager.OnActivateSkull += ActivateSkull;

		ClearSkulls();

		var alreadyActiveSkulls = skullManager.activeSkulls;
		foreach (var skull in alreadyActiveSkulls)
		{
			ActivateSkull(skull);
		}
	}

	public void ClearSkulls()
	{
		foreach (var ui_skull in ui_skulls)
		{
			ui_skull.ClearSkull();
		}
		index = 0;
	}

	public void ActivateSkull(Skull skull)
	{
		if (index >= ui_skulls.Length) return;
		ui_skulls[index].SetSkull(skull);
		index++;
		if (index >= ui_skulls.Length) index = 0; // Reset index if it exceeds the array length
	}
}
