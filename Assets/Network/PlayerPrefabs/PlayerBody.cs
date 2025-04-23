using Fusion;
using System;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerBody : NetworkBehaviour
{


    [Header("References")]
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerAim playerAim;
    [SerializeField] ArmsExtended playerArms;
    [SerializeField] Transform head;


    [SerializeField] GameObject mainMesh;
    [SerializeField] SkinnedMeshRenderer[] meshes;

    [SerializeField] PlayerPickUpScan playerPickUpScan;
    [SerializeField] Health health;
    [SerializeField] BulletSpawner bulletSpawner;
    [SerializeField] Transform spectatorCameraTarget;
    [SerializeField] WeaponInventoryExtended weaponInventory;
    [SerializeField] AbilityInventory abilityInventory;
    [SerializeField] TargetHitCollector targetHitCollector;
    [SerializeField] PlayerTeam playerTeam;
    [SerializeField] PlayerStartEquipment playerStartEquipment;
    [SerializeField] PlayerAnimation playerAnimation;
    [SerializeField] GranadeThrower granadeThrower;

    [Networked] int localPlayerIndex { get; set; }
    int visualLayerIndex = 0;

    public void SetVisualLayer(int visualLayer)
    {
        visualLayerIndex = visualLayer;

        UtilityFunctions.SetLayerRecursively(mainMesh, visualLayerIndex);
    }

    public int LocalPlayerIndex
    {
        get => localPlayerIndex;
        set
        {
            localPlayerIndex = value;
        }
    }

    public void SetCameras(CinemachineCamera camera, CinemachineCamera spectatorCamera)
    {
        camera.Follow = head.transform;
        camera.LookAt = head.transform;

        //mind.SetSpectatorTarget(spectatorCamera);
        spectatorCamera.Follow = transform;
        spectatorCamera.LookAt = spectatorCameraTarget;
    }


    public PlayerMovement PlayerMovement => playerMovement;
    public PlayerAim PlayerAim => playerAim;
    public ArmsExtended PlayerArms => playerArms;
    public PlayerPickUpScan PlayerPickUpScan => playerPickUpScan;
    public Health Health => health;
    public BulletSpawner BulletSpawner => bulletSpawner;
    public Transform SpectatorCameraTarget => spectatorCameraTarget;
    public WeaponInventoryExtended WeaponInventory => weaponInventory;
    public AbilityInventory AbilityInventory => abilityInventory;
    public TargetHitCollector TargetHitCollector => targetHitCollector;
    public PlayerTeam PlayerTeam => playerTeam;
    public PlayerStartEquipment PlayerStartEquipment => playerStartEquipment;
    public PlayerAnimation PlayerAnimation => playerAnimation;
    public SkinnedMeshRenderer[] Meshes => meshes;
    public GameObject MainMesh => mainMesh;

    public Transform HeadTransform => head;

    public GranadeThrower GranadeThrower => granadeThrower;

}
