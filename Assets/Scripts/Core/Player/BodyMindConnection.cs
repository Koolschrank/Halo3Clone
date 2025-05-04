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
    [SerializeField] PlayerStartEquipment playerStartEquipment;
    [SerializeField] PlayerAnimation playerAnimation;

    [SerializeField] SkinnedMeshRenderer[] meshes;


    PlayerMind mind;

    public void ConnectMind(PlayerMind mind)
    {
        

        this.mind = mind;
        mind.SetPlayerBody(gameObject);
        mind.transform.SetParent(mindParent);
        mind.SetPlayerModel(mesh);
        mind.SetPlayerMovement(playerMovement);
        mind.SetPlayerAim(playerAim);
        mind.SetPlayerArms(playerArms);
        mind.SetHealth(health);
        mind.SetPickUpScan(playerPickUpScan);
        mind.SetBulletSpawner(bulletSpawner);
        
        mind.SetPlayerInventory(playerInventory);
        mind.transform.localPosition = Vector3.zero;
        mind.transform.localRotation = Quaternion.identity;

        

        mind.ConnectPlayerElimination(targetHitCollector);
        SetPlayTeamIndex();
        playerStartEquipment.GetEquipment(GetStartEquipment());


    }

    public Equipment GetStartEquipment()
    {
        var equipment = new Equipment( GameModeSelector.gameModeManager.GetEquipment());
        bool isSwat = MapLoader.instance.IsSwat();
        bool dualWieldPlus = MapLoader.instance.IsDualWieldPlus();
        bool noMiniMap = MapLoader.instance.HasNoMiniMap();
        bool randomWeaponSpawn = MapLoader.instance.IsRandomWeaponSpawn();
        float moveSpeedMultiplier = MapLoader.instance.GetMoveSpeedMultiplier();

        if (isSwat)
        {
            equipment.SetWeapons(ItemList.instance.GetPistol(), null, null);
            equipment.SetMagazins(4, 0, 0);
            equipment.SetHasShild(false);
            equipment.SetHeadShotOneShot(false);
        }
        if (dualWieldPlus)
        {
            equipment.SetCanDualWieldEverything(true);
        }
        if (noMiniMap)
        {
            equipment.SetHasMiniMap(false);
        }
        if (randomWeaponSpawn) {
            equipment.SetWeapons(ItemList.instance.GetRandomWeapon(), null, null);
            equipment.SetMagazins(3, 0, 0);
        }

        if (moveSpeedMultiplier != 1)
        {
            equipment.SetMovementSpeedMultiplier(moveSpeedMultiplier);
        }




        return equipment;
    }


    public void SetCameras(CinemachineCamera camera, CinemachineCamera spectatorCamera)
    {
        camera.Follow = mindParent.transform;
        camera.LookAt = mindParent.transform;
        mind.SetSpectatorTarget(spectatorCamera);
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
