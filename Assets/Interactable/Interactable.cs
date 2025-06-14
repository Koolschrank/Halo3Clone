using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public bool HasPrice;
    [SerializeField] int BasePrice = 0;
    public string discription = "Interactable Object";
    public string extraDiscription = "";
    int currentPrice = 0;

    public bool isInteractable = true;

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
        if (HasPrice)
            PayPrice(player);

    }

    public virtual void PayPrice(GameObject player)
    {
        var body = player.GetComponent<BodyMindConnection>();
        body.Mind.LooseScore(currentPrice);

    }
}
