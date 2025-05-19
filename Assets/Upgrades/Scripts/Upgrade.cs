using UnityEngine;

public abstract class Upgrade : ScriptableObject
{
    public string UpgradeName;
    [Multiline]
    public string Description;
    public Sprite Icon;


    public virtual void Apply(GameObject body)
    {

    }
}
