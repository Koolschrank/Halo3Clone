using Unity.Cinemachine;
using UnityEngine;

public class BodyMindConnection : MonoBehaviour
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

    [SerializeField] SkinnedMeshRenderer[] meshes;


    PlayerMind mind;

    public void ConnectMind(PlayerMind mind, CinemachineCamera camera, CinemachineCamera spectatorCamera)
    {
        camera.Follow = mindParent.transform;
        camera.LookAt = mindParent.transform;

        this.mind = mind;

        mind.transform.SetParent(mindParent);
        mind.SetPlayerModel(mesh);
        mind.SetPlayerMovement(playerMovement);
        mind.SetPlayerAim(playerAim);
        mind.SetPlayerArms(playerArms);
        mind.SetHealth(health);
        mind.SetPickUpScan(playerPickUpScan);
        mind.SetBulletSpawner(bulletSpawner);
        mind.SetSpectatorTarget(spectatorCamera);
        mind.SetPlayerInventory(playerInventory);
        mind.transform.localPosition = Vector3.zero;
        mind.transform.localRotation = Quaternion.identity;

        spectatorCamera.Follow = transform  ;
        spectatorCamera.LookAt = spectatorCameraTarget;

        mind.ConnectPlayerElimination(targetHitCollector);
        SetPlayTeamIndex();

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

    public PlayerMind GetMind()
    {
        return mind;
    }
}
