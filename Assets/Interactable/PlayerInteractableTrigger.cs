using NUnit.Framework;
using System;
using UnityEngine;

public class PlayerInteractableTrigger : MonoBehaviour
{
    public Action<Interactable> OnNewInteractable;
    public Action OnRemoveInteractable;



    [SerializeField] float interactionRadius = 3f;
    [SerializeField] float interactionDistance = 2f;
    [SerializeField] LayerMask interactableLayerMask;
    [SerializeField] float interactionCooldown = 0.5f; // Cooldown for interaction to prevent spamming
    float lastInteractionTime = 0f;

    Interactable currentInteractable;

    [SerializeField] BodyMindConnection body;
    PlayerMind mind;


    private void Start()
    {
        mind = body.Mind;
        if (mind == null)
        {
            this.enabled = false;
        }
    }



    private void Update()
    {

        // Check if the interaction cooldown has passed
        if (Time.time - lastInteractionTime < interactionCooldown)
        {
            return; // Exit if still in cooldown
        }


        // make a sphere cast in front of the player to find interactable objects
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, interactionRadius, transform.forward, out hit, interactionDistance, interactableLayerMask))
        {
            var interactable = hit.collider.GetComponent<Interactable>();
            if (interactable != null && interactable != currentInteractable)
            {
                if (interactable.CanUse(body.gameObject))
                {
                    SetNewInteractable(interactable);
                }
                else
                {
                    RemoveInteractable();
                }


                
            }
            
        }
        else
        {
            if (currentInteractable != null)
            {
                RemoveInteractable();
            }
        }
        

    }


    public bool CanInteract()
    {
        if (currentInteractable == null)
            return false;
        if (mind == null || (currentInteractable.HasPrice && mind.Score < currentInteractable.Price))
            return false;

        return true;
    }

    public void Interact()
    {
        currentInteractable.Interact(body.gameObject);
        RemoveInteractable(); // Remove interactable after interaction
        lastInteractionTime = Time.time; // Reset interaction cooldown timer
    }




    void SetNewInteractable(Interactable interactable)
    {
        if (currentInteractable != null)
        {
            RemoveInteractable();
        }
        currentInteractable = interactable;
        OnNewInteractable?.Invoke(currentInteractable);

    }

    void RemoveInteractable()
    {
        if (currentInteractable != null)
        {
            OnRemoveInteractable?.Invoke();
            currentInteractable = null;
            
        }

    }
}
