using UnityEngine;
using UnityEngine.Rendering;

public class AI_Target : MonoBehaviour
{
    GameObject target;

    [SerializeField] bool alwaysKnowsWherePlayerIs = false;
    [SerializeField] int framesToCheckForNewTarget = 100;



    public void AssignToClosesPlayer()
    {
        // get closest player
        var players = PlayerManager.instance.GetAllPlayers();
        float closestDistance = Mathf.Infinity;
        foreach (var player in players)
        {
            if (player != null && player.gameObject.activeInHierarchy)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    target = player.PlayerBody.gameObject;
                    Debug.Log("Target assigned to: " + target.name);
                }
            }
        }
    }

    private void Awake()
    {
        AssignToClosesPlayer();
    }

    private void Update()
    {
        

        if (alwaysKnowsWherePlayerIs && Time.frameCount % framesToCheckForNewTarget ==0)
        {
            AssignToClosesPlayer();
        }
    }

    public void AssignTarget(Transform target)
    {
        this.target = target.gameObject;
    }

    public Vector3 GetTargetPosition()
    {
        if (target != null)
        {
            return target.transform.position;
        }
        else
        {
            Debug.LogWarning("Target is not assigned.");
            return Vector3.zero;
        }
    }
}
