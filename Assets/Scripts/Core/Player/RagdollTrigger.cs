using UnityEngine;
using Fusion.Addons.KCC;
using Fusion;

public class RagdollTrigger : MonoBehaviour
{
    [SerializeField] GameObject ragdollPrefab;


    //[SerializeField] Animator animator;
    //[SerializeField] Collider[] colliders;
    [SerializeField] KCC cc;

    [SerializeField] Transform mesh;

    //[SerializeField] PlayerArms playerArms;
    //[SerializeField] PlayerMovement playerMovement;
    //[SerializeField] bool enableRagdollAtStart = false;

    //bool inRagdollMode = false;


    //private void Awake()
    //{
    //    GetRagdollBits();
    //    if (enableRagdollAtStart)
    //    {
    //        RagdollModeOn();
    //    }
    //    else
    //    {
    //        RagdollModeOff();
    //    }

    //}

    //public void SwitchMode()
    //{
    //    if (inRagdollMode)
    //    {
    //        RagdollModeOff();
    //    }
    //    else
    //    {
    //        RagdollModeOn();
    //    }
    //}

    public void Activate(RagddollImpactStruct damagePackage)
    {

        Vector3 velocity = cc.Data.RealVelocity;

        // spawn ragdoll
        var ragdoll = Instantiate(ragdollPrefab, transform.position, transform.rotation);
        var ragdollComponents = ragdoll.GetComponentsInChildren<Rigidbody>();
        var ownComponents = mesh.GetComponentsInChildren<Rigidbody>();

        // copy position and rotation
        for (int i = 0; i < ragdollComponents.Length; i++)
        {
            ragdollComponents[i].position = ownComponents[i].position;
            ragdollComponents[i].rotation = ownComponents[i].rotation;
            ragdollComponents[i].linearVelocity = velocity;
        }


        // apply velocity

        Debug.Log(damagePackage.force);
        if (damagePackage.singleBodyPart)
        {
            Rigidbody closest = GetClosesRigidbody(damagePackage.hitPoint, ragdollComponents);
            closest.AddForceAtPosition(damagePackage.force, damagePackage.hitPoint, ForceMode.Impulse);
        }
        else
        {
            foreach (Rigidbody rb in ragdollComponents)
            {
                rb.AddForce(damagePackage.force, ForceMode.Impulse);
            }

        }

        UtilityFunctions.SetLayerRecursively(ragdoll.gameObject, PlayerManager.instance.GetDeadPlayerLayer());


        //inRagdollMode = true;
        //animator.enabled = false;
        //cc.enabled = false;
        //playerArms.enabled = false;
        //playerMovement.enabled = false;

        //foreach (Collider c in colliders)
        //{
        //    c.enabled = false;
        //}



        //foreach (Rigidbody rb in ragdollRigidbodies)
        //{
        //    rb.isKinematic = false;
        //}

        //foreach (Collider c in ragdollColliders)
        //{
        //    c.enabled = true;
        //}

        //Vector3 velocity = cc.Data.RealVelocity;
        //foreach (Rigidbody rb in ragdollRigidbodies)
        //{
        //    rb.linearVelocity = cc.Data.RealVelocity;
        //}

        //if (damagePackage.singleBodyPart)
        //{
        //    Rigidbody closest = GetClosesRigidbody(damagePackage.hitPoint);
        //    closest.AddForceAtPosition(damagePackage.force, damagePackage.hitPoint, ForceMode.Impulse);
        //}
        //else
        //{
        //    foreach (Rigidbody rb in ragdollRigidbodies)
        //    {
        //        rb.AddForce(damagePackage.force, ForceMode.Impulse);
        //    }

        //}

        // TODO: not best way to handle this
        


    }

    //public void Activate(Vector3 extraForce, Vector3 extraForcePosition, bool impactOnlyOneBodyPart)
    //{
    //    inRagdollMode = true;
    //    animator.enabled = false;
    //    cc.enabled = false;
    //    playerArms.enabled = false;
    //    playerMovement.enabled = false;

    //    foreach (Collider c in colliders)
    //    {
    //        c.enabled = false;
    //    }



    //    foreach (Rigidbody rb in ragdollRigidbodies)
    //    {
    //        rb.isKinematic = false;
    //    }

    //    foreach (Collider c in ragdollColliders)
    //    {
    //        c.enabled = true;
    //    }

    //    Vector3 velocity = cc.Data.RealVelocity;
    //    foreach (Rigidbody rb in ragdollRigidbodies)
    //    {
    //        rb.linearVelocity = velocity;
    //    }

    //    if (impactOnlyOneBodyPart)
    //    {
    //        Rigidbody closest = GetClosesRigidbody(extraForcePosition);
    //        closest.AddForceAtPosition(extraForce, extraForcePosition, ForceMode.Impulse);
    //    }
    //    else
    //    {
    //        foreach (Rigidbody rb in ragdollRigidbodies)
    //        {
    //            rb.AddForce(extraForce, ForceMode.Impulse);
    //        }

    //    }

    //    // TODO: not best way to handle this
    //    UtilityFunctions.SetLayerRecursively(mesh.gameObject, PlayerManager.instance.GetDeadPlayerLayer());

    //}

    //public void Activate(Vector3 extraForce)
    //{
    //    inRagdollMode = true;
    //    animator.enabled = false;
    //    cc.enabled = false;
    //    playerArms.enabled = false;
    //    playerMovement.enabled = false;

    //    foreach (Collider c in colliders)
    //    {
    //        c.enabled = false;
    //    }



    //    foreach (Rigidbody rb in ragdollRigidbodies)
    //    {
    //        rb.isKinematic = false;
    //    }

    //    foreach (Collider c in ragdollColliders)
    //    {
    //        c.enabled = true;
    //    }

    //    Vector3 velocity = cc.Data.RealVelocity + extraForce;
    //    foreach (Rigidbody rb in ragdollRigidbodies)
    //    {
    //        rb.linearVelocity = velocity;
    //    }

    //    // TODO: not best way to handle this
    //    UtilityFunctions.SetLayerRecursively(mesh.gameObject, PlayerManager.instance.GetDeadPlayerLayer());
    //}

    public Rigidbody GetClosesRigidbody(Vector3 position, Rigidbody[] bodies )
    {
        Rigidbody closest = null;
        float closestDistance = float.MaxValue;
        foreach (Rigidbody rb in bodies)
        {
            float distance = Vector3.Distance(rb.position, position);
            if (distance < closestDistance)
            {
                closest = rb;
                closestDistance = distance;
            }
        }
        return closest;
    }

    //public Rigidbody GetClosesRigidbody(Vector3 position)
    //{
    //    Rigidbody closest = null;
    //    float closestDistance = float.MaxValue;
    //    foreach (Rigidbody rb in ragdollRigidbodies)
    //    {
    //        float distance = Vector3.Distance(rb.position, position);
    //        if (distance < closestDistance)
    //        {
    //            closest = rb;
    //            closestDistance = distance;
    //        }
    //    }
    //    return closest;
    //}

    //void RagdollModeOn()
    //{
    //    Activate(Vector3.zero);
    //}

    //void RagdollModeOff()
    //{
    //    inRagdollMode = false;
    //    animator.enabled = true;
    //    cc.enabled = true;
    //    playerArms.enabled = true;
    //    playerMovement.enabled = true;
    //    foreach (Collider c in colliders)
    //    {
    //        c.enabled = true;
    //    }
    //    foreach (Rigidbody rb in ragdollRigidbodies)
    //    {
    //        rb.isKinematic = true;
    //    }
    //    foreach (Collider c in ragdollColliders)
    //    {
    //        c.enabled = false;
    //    }
    //}


    //Collider[] ragdollColliders;
    //Rigidbody[] ragdollRigidbodies;
    //public void GetRagdollBits()
    //{
    //    ragdollColliders = mesh.GetComponentsInChildren<Collider>();
    //    ragdollRigidbodies = mesh.GetComponentsInChildren<Rigidbody>();
    //}


}


public struct RagddollImpactStruct : INetworkStruct
{
    public bool singleBodyPart;
    public Vector3 force;
    public Vector3 hitPoint;

}