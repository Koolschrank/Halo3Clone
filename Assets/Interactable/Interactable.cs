using System;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public bool HasPrice;
    [SerializeField] int BasePrice = 0;
    public string discription = "Interactable Object";
    public string extraDiscription = "";
    protected int currentPrice = 0;

    public bool isInteractable = true;

    [SerializeField] Interactable[] connectedInteractables;
    

    [NonSerialized]
    public bool inInteraction = false;

    protected virtual void Awake()
    {
        if (HasPrice)
        {
            currentPrice = BasePrice;
        }
    }

    public int Price => currentPrice;


    public virtual bool CanUse(GameObject player)
    {
        return isInteractable;
    }

    public bool CanAfford(int playerMoney)
    {
        return !HasPrice || playerMoney >= currentPrice;
    }

   

    public virtual void Interact(GameObject player)
    {

        if (HasPrice && !IsConnectedInteractableInInteraction())
            PayPrice(player);

        InteractAllConnectedObjects(player);
    }

    void InteractAllConnectedObjects(GameObject player)
    {
        inInteraction = true;
        foreach (var interactable in connectedInteractables)
        {
            if (interactable != null && !interactable.inInteraction)
            {
                interactable.Interact(player);
            }
        }
        inInteraction = false;
    }

    public bool IsConnectedInteractableInInteraction()
    {
        foreach (var interactable in connectedInteractables)
        {
            if (interactable != null && interactable.inInteraction)
            {
                return true;
            }
        }
        return false;
    }

    public virtual void PayPrice(GameObject player)
    {
        var body = player.GetComponent<BodyMindConnection>();
        body.Mind.LooseScore(currentPrice);

    }
}
