using Fusion;
using Unity.Cinemachine;
using UnityEngine;

public class BodyMindConnection : NetworkBehaviour
{
    [SerializeField] Transform mindParent;
    [SerializeField] GameObject mesh;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerAim playerAim;
    [SerializeField] PlayerArms playerArms;
    [SerializeField] PlayerPickUpScan playerPickUpScan;
    [SerializeField] Health health;
    [SerializeField] BulletSpawner bulletSpawner;
    [SerializeField] Transform spectatorCameraTarget;
    [SerializeField] PlayerInventory playerInventory;
    [SerializeField] TargetHitCollector targetHitCollector;
    [SerializeField] PlayerTeam playerTeam;
    [SerializeField] PlayerStartEquipment playerStartEquipment;
    [SerializeField] PlayerAnimation playerAnimation;

    [SerializeField] SkinnedMeshRenderer[] meshes;


    PlayerMind mind;

    public void ConnectMind(PlayerMind mind)
    {
        

        this.mind = mind;
       
        mind.transform.localPosition = Vector3.zero;
        mind.transform.localRotation = Quaternion.identity;

        

        mind.ConnectPlayerElimination(targetHitCollector);
        SetPlayTeamIndex();
        playerStartEquipment.GetEquipment(GameModeSelector.gameModeManager.GetEquipment());


    }

    public void SetCameras(CinemachineCamera camera, CinemachineCamera spectatorCamera)
    {
        camera.Follow = mindParent.transform;
        camera.LookAt = mindParent.transform;
        //mind.SetSpectatorTarget(spectatorCamera);
        spectatorCamera.Follow = transform;
        spectatorCamera.LookAt = spectatorCameraTarget;


    }

    public void SetPlayTeamIndex()
    {
        playerTeam.SetTeamIndex(mind.TeamIndex);
    }

    public void SetMaterial(Material material)
    {
        foreach (var mesh in meshes)
        {
            mesh.material = material;
        }
    }

    public void SetPlayerColor(Color color)
    {
        playerAnimation.SetPlayerColor(color);
    }

    public PlayerMind GetMind()
    {
        return mind;
    }
}
