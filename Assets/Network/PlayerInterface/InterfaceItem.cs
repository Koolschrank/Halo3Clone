using UnityEngine;

public class InterfaceItem : MonoBehaviour
{
    [SerializeField] PlayerInterface playerInterface;
    PlayerBody body;

    protected virtual void Awake()
    {
        if (playerInterface == null)
        {
            playerInterface = GetComponentInParent<PlayerInterface>();
        }
        if (playerInterface != null)
        {
            playerInterface.OnPlayerBodySet += SetUp;
        }
    }

    void SetUp(PlayerBody character)
    {
        if (body != null)
        {
            Unsubscribe(body);
        }
        body = character;
        if (body != null)
        {
            Subscribe(body);
        }
    }

    protected virtual void Unsubscribe(PlayerBody body)
    {
        
    }

    protected virtual void Subscribe(PlayerBody body)
    {

    }


}
