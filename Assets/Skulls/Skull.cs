using UnityEngine;

public class Skull : ScriptableObject
{
    public string skullName;
    [TextArea]
    public string skullDescription;
    public Sprite skullIcon;
    public Color skullColor = Color.white;

	public virtual void Activate()
    {

    }

    public virtual void Deactivate()
    {

	}

    public virtual void PlayerSpawned(PlayerMind player)
    {

    }
}
