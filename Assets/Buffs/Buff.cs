using UnityEngine;

public abstract class Buff : ScriptableObject
{
    public float duration = 30f;

    public Color buffColor = Color.white;

    public abstract void ApplyBuff(GameObject player);

    public abstract void RemoveBuff(GameObject player);
}
