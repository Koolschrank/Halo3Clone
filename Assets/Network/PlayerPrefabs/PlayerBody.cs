using Fusion;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerBody : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerAim playerAim;
    [SerializeField] PlayerArms playerArms;
    [SerializeField] Transform head;


    [SerializeField] GameObject mainMesh;
    [SerializeField] SkinnedMeshRenderer[] meshes;

    [SerializeField] PlayerPickUpScan playerPickUpScan;
    [SerializeField] Health health;
    [SerializeField] BulletSpawner bulletSpawner;
    [SerializeField] Transform spectatorCameraTarget;
    [SerializeField] PlayerInventory playerInventory;
    [SerializeField] TargetHitCollector targetHitCollector;
    [SerializeField] PlayerTeam playerTeam;
    [SerializeField] PlayerStartEquipment playerStartEquipment;
    [SerializeField] PlayerAnimation playerAnimation;

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
        Debug.Log("Setting cameras");
        camera.Follow = head.transform;
        camera.LookAt = head.transform;
        //mind.SetSpectatorTarget(spectatorCamera);
        spectatorCamera.Follow = transform;
        spectatorCamera.LookAt = spectatorCameraTarget;
    }


    public PlayerMovement PlayerMovement => playerMovement;
    public PlayerAim PlayerAim => playerAim;
    public PlayerArms PlayerArms => playerArms;
    public PlayerPickUpScan PlayerPickUpScan => playerPickUpScan;
    public Health Health => health;
    public BulletSpawner BulletSpawner => bulletSpawner;
    public Transform SpectatorCameraTarget => spectatorCameraTarget;
    public PlayerInventory PlayerInventory => playerInventory;
    public TargetHitCollector TargetHitCollector => targetHitCollector;
    public PlayerTeam PlayerTeam => playerTeam;
    public PlayerStartEquipment PlayerStartEquipment => playerStartEquipment;
    public PlayerAnimation PlayerAnimation => playerAnimation;
    public SkinnedMeshRenderer[] Meshes => meshes;
    public GameObject MainMesh => mainMesh;

    public Transform HeadTransform => head;

}
