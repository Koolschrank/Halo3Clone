using UnityEngine;
using Fusion.Addons.SimpleKCC;

public class RagdollTrigger : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Collider[] colliders;
    [SerializeField] SimpleKCC cc;

    [SerializeField] Transform ragdoll;

    [SerializeField] PlayerArms playerArms;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] bool enableRagdollAtStart = false;

    bool inRagdollMode = false;


    private void Awake()
    {
        GetRagdollBits();
        if (enableRagdollAtStart)
        {
            RagdollModeOn();
        }
        else
        {
            RagdollModeOff();
        }

    }

    public void SwitchMode()
    {
        if (inRagdollMode)
        {
            RagdollModeOff();
        }
        else
        {
            RagdollModeOn();
        }
    }

    public void Activate(DamagePackage damagePackage)
    {
        inRagdollMode = true;
        animator.enabled = false;
        cc.enabled = false;
        playerArms.enabled = false;
        playerMovement.enabled = false;

        foreach (Collider c in colliders)
        {
            c.enabled = false;
        }



        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = false;
        }

        foreach (Collider c in ragdollColliders)
        {
            c.enabled = true;
        }

        Vector3 velocity = cc.RealVelocity;
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.linearVelocity = cc.RealVelocity;
        }

        if (damagePackage.impactType == ImpactType.singleBodyPart)
        {
            Rigidbody closest = GetClosesRigidbody(damagePackage.hitPoint);
            closest.AddForceAtPosition(damagePackage.forceVector, damagePackage.hitPoint, ForceMode.Impulse);
        }
        else
        {
            foreach (Rigidbody rb in ragdollRigidbodies)
            {
                rb.AddForce(damagePackage.forceVector, ForceMode.Impulse);
            }

        }

        // TODO: not best way to handle this
        UtilityFunctions.SetLayerRecursively(ragdoll.gameObject, PlayerManager.instance.GetDeadPlayerLayer());


    }

    public void Activate(Vector3 extraForce, Vector3 extraForcePosition, bool impactOnlyOneBodyPart)
    {
        inRagdollMode = true;
        animator.enabled = false;
        cc.enabled = false;
        playerArms.enabled = false;
        playerMovement.enabled = false;

        foreach (Collider c in colliders)
        {
            c.enabled = false;
        }



        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = false;
        }

        foreach (Collider c in ragdollColliders)
        {
            c.enabled = true;
        }

        Vector3 velocity = cc.RealVelocity;
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.linearVelocity = velocity;
        }

        if (impactOnlyOneBodyPart)
        {
            Rigidbody closest = GetClosesRigidbody(extraForcePosition);
            closest.AddForceAtPosition(extraForce, extraForcePosition, ForceMode.Impulse);
        }
        else
        {
            foreach (Rigidbody rb in ragdollRigidbodies)
            {
                rb.AddForce(extraForce, ForceMode.Impulse);
            }

        }

        // TODO: not best way to handle this
        UtilityFunctions.SetLayerRecursively(ragdoll.gameObject, PlayerManager.instance.GetDeadPlayerLayer());

    }

    public void Activate(Vector3 extraForce)
    {
        inRagdollMode = true;
        animator.enabled = false;
        cc.enabled = false;
        playerArms.enabled = false;
        playerMovement.enabled = false;

        foreach (Collider c in colliders)
        {
            c.enabled = false;
        }



        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = false;
        }

        foreach (Collider c in ragdollColliders)
        {
            c.enabled = true;
        }

        Vector3 velocity = cc.RealVelocity + extraForce;
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.linearVelocity = velocity;
        }

        // TODO: not best way to handle this
        UtilityFunctions.SetLayerRecursively(ragdoll.gameObject, PlayerManager.instance.GetDeadPlayerLayer());
    }

    public Rigidbody GetClosesRigidbody(Vector3 position)
    {
        Rigidbody closest = null;
        float closestDistance = float.MaxValue;
        foreach (Rigidbody rb in ragdollRigidbodies)
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

    void RagdollModeOn()
    {
        Activate(Vector3.zero);
    }

    void RagdollModeOff()
    {
        inRagdollMode = false;
        animator.enabled = true;
        cc.enabled = true;
        playerArms.enabled = true;
        playerMovement.enabled = true;
        foreach (Collider c in colliders)
        {
            c.enabled = true;
        }
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = true;
        }
        foreach (Collider c in ragdollColliders)
        {
            c.enabled = false;
        }
    }


    Collider[] ragdollColliders;
    Rigidbody[] ragdollRigidbodies;
    public void GetRagdollBits()
    {
        ragdollColliders = ragdoll.GetComponentsInChildren<Collider>();
        ragdollRigidbodies = ragdoll.GetComponentsInChildren<Rigidbody>();
    }


}
