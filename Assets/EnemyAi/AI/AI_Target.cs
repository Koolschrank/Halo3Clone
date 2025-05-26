using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class AI_Target : MonoBehaviour
{
    GameObject target;

    [SerializeField] bool alwaysKnowsWherePlayerIs = false;
    [SerializeField] int framesToCheckForNewTarget = 100;


    [SerializeField] bool CheckForAIEnemies = false;
    [SerializeField] PlayerTeam team;
    

    public void Awake()
    {
       
        if (EnemySpawner.instance.IsAutoActiveOnThisMap)
        {
            CheckForAIEnemies = false;
        }
        AssignToClosesTarget();
    }


    public List<GameObject> GetAllPossibleTargets()
    {
        List<GameObject> validTargets = new List<GameObject>();

        var players = PlayerManager.instance.GetAllPlayers();

        foreach (var player in players)
        {
            if (player != null && player.gameObject.activeInHierarchy && !player.IsDead && player.GetComponent<PlayerTeam>().TeamIndex != team.TeamIndex)
            {
                validTargets.Add(player.PlayerBody.gameObject);
            }
        }

        if (CheckForAIEnemies)
        {
            var enemies = EnemySpawner.instance.activeEnemies;
            foreach (var enemy in enemies)
            {
                if (enemy != null && enemy.gameObject.activeInHierarchy && !enemy.GetComponent<CharacterHealth>().IsDead && enemy.GetComponent<PlayerTeam>().TeamIndex != team.TeamIndex)
                {
                    validTargets.Add(enemy.gameObject);
                }
            }

        }


        return validTargets;


    }


    public void AssignToClosesTarget()
    {

        List<GameObject> validTargets = GetAllPossibleTargets();





        float closestDistance = Mathf.Infinity;
        GameObject closestTarget = null;
        foreach (var player in validTargets)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = player;
            }
        }
        if (closestTarget != null)
        {
            target = closestTarget;
        }
        else
        {
            Debug.LogWarning("No valid targets found.");
        }
    }


    private void Update()
    {
        

        if (alwaysKnowsWherePlayerIs && Time.frameCount % framesToCheckForNewTarget ==0)
        {
            AssignToClosesTarget();
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

    public bool IsTargetPlayerMind => target != null && target.GetComponent<PlayerMind>() != null;
}
