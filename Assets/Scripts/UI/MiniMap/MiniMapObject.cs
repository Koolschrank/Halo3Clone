using UnityEngine;

public class MiniMapObject : MonoBehaviour
{
    [SerializeField] GameObject icon_Ally;
    [SerializeField] GameObject icon_Enemy;
    [SerializeField] PlayerTeam playerTeam;
    [SerializeField] CharacterHealth characterHealth;

    private void Start()
    {
        AddSelfToMapManager();
        characterHealth.OnDeath += RemoveSelfFromMapManager;
    }

    public void AddSelfToMapManager()
    {
        MiniMapManager.instance.AddMinimapObject(this);
    }

    public void RemoveSelfFromMapManager()
    {
        MiniMapManager.instance.RemoveMinimapObject(this);
    }

    public GameObject GetIcon(int playerIndex)
    {
        if (playerTeam.TeamIndex == playerIndex)
        {
            return icon_Ally;
        }
        else
        {
            return icon_Enemy;
        }
    }


}
