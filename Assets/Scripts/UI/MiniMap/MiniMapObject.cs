using UnityEngine;

public class MiniMapObject : MonoBehaviour
{
    [SerializeField] GameObject icon_Ally;
    [SerializeField] GameObject icon_Enemy;
    [SerializeField] PlayerTeam playerTeam;
    [SerializeField] CharacterHealth characterHealth;


    [SerializeField] CharacterController characterController;
    [SerializeField] bool isHiddenOnMiniMap = false;
    [SerializeField] float movementSpeedForVisibility = 0.1f; // Speed threshold to determine visibility on the minimap
    public bool IsHiddenOnMiniMap => isHiddenOnMiniMap;

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

    public void Update()
    {
        if (characterController != null && characterController.velocity.magnitude > movementSpeedForVisibility)
        {
            MakeVisible();
        }
        else
        {
            MakeHidden();
        }
    }

    public void MakeVisible()
    {
        isHiddenOnMiniMap = false;
    }

    public void MakeHidden()
    {
        isHiddenOnMiniMap = true;
    }


}
