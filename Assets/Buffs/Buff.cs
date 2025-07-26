using UnityEngine;

public abstract class Buff : ScriptableObject
{
    public string buffName = "Buff Name";
	public float duration = 30f;

    public Color buffColor = Color.white;
    public bool nonTimedBuff = false;

	public abstract void ApplyBuff(GameObject player);

    public abstract void RemoveBuff(GameObject player);
}
