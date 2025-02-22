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

    [SerializeField] SkinnedMeshRenderer[] meshes;

    public void ConnectMind(PlayerMind mind)
    {
        mind.transform.SetParent(mindParent);
        mind.SetPlayerModel(mesh);
        mind.SetPlayerMovement(playerMovement);
        mind.SetPlayerAim(playerAim);
        mind.SetPlayerArms(playerArms);
        mind.SetHealth(health);
        mind.SetPickUpScan(playerPickUpScan);
        mind.SetBulletSpawner(bulletSpawner);
        mind.SetSpectatorTarget(spectatorCameraTarget);
        mind.SetPlayerInventory(playerInventory);
        mind.transform.localPosition = Vector3.zero;
        mind.transform.localRotation = Quaternion.identity;

    }

    public void SetMaterial(Material material)
    {
        foreach (var mesh in meshes)
        {
            mesh.material = material;
        }
    }
}
