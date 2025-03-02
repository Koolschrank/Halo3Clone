using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicatorUI : MonoBehaviour
{
    [SerializeField] float damageIndicatorTime;
    [SerializeField] float damageIndicatorDistance;
    [SerializeField] GameObject hitIndicatorPrefab;
    [SerializeField] float hideZone = 50;
    

    List<DamageIndicator> damageIndicators = new List<DamageIndicator>();
    

    public void AddDamageIndicator(DamagePackage damagePackage)
    {
        
        if (!CheckIfHitIndicatorIsNeeded(damagePackage.origin, transform))
        {
            return;
        }


        // spawn hit prefab as child
        var hitVisual = Instantiate(hitIndicatorPrefab) as GameObject;
        hitVisual.transform.SetParent(transform, false);



        damageIndicators.Add(new DamageIndicator(damagePackage, damageIndicatorTime, hitVisual));

    }

    public void Update()
    {
        List<int> indexesToRemove = new List<int>();
        for (int i = 0; i < damageIndicators.Count; i++)
        {
            damageIndicators[i].Update(transform, damageIndicatorDistance);
            if (damageIndicators[i].IsFinished)
            {
                indexesToRemove.Add(i);
            }
        }
        for (int i = indexesToRemove.Count - 1; i >= 0; i--)
        {
            damageIndicators[i].DestroyVisual();

            damageIndicators.RemoveAt(indexesToRemove[i]);
        }



    }

    public bool CheckIfHitIndicatorIsNeeded(Vector3 enemyPosition, Transform playerTransform)
    {
        Vector3 directionToEnemy = (enemyPosition - playerTransform.position).normalized;

        // Project onto XZ plane
        Vector3 flatDirection = new Vector3(directionToEnemy.x, 0, directionToEnemy.z).normalized;

        // Convert world direction to local space relative to player's forward direction
        float forwardDot = Vector3.Dot(flatDirection, playerTransform.forward); // Front/Back
        float rightDot = Vector3.Dot(flatDirection, playerTransform.right);     // Right/Left

        // Convert to UI coordinates
        Vector2 uiDirection = new Vector2(rightDot, forwardDot).normalized;

        float angle = Mathf.Atan2(uiDirection.y, uiDirection.x) * Mathf.Rad2Deg;

        // Rotate by 90 degrees to make it point outward (arrows usually point right at 0°)
        angle -= 90f;
        return Mathf.Abs(angle) > hideZone;
    }

    // on disable remove all indicators
    public void OnDisable()
    {
        foreach (var indicator in damageIndicators)
        {
            indicator.DestroyVisual();
        }
        damageIndicators.Clear();
    }

}

public class DamageIndicator
{
    DamagePackage damagePackage;
    float lifeTime;
    float lifeTime_left;
    GameObject visual;

    public DamageIndicator(DamagePackage damagePackage, float lifeTime, GameObject visual)
    {
        this.damagePackage = damagePackage;
        this.lifeTime = lifeTime;
        this.visual = visual;
        lifeTime_left = lifeTime;
        visual.SetActive(false);
    }

    public Vector3 Origin => damagePackage.origin;

    public void Update(Transform playerTransform, float distance)
    {
        if (lifeTime_left == lifeTime)
        {
            visual.SetActive(true);
        }

        lifeTime_left -= Time.deltaTime;

        Vector3 direction = Origin - playerTransform.position;
        direction.y = 0;
        direction.Normalize();
        Vector2 indicatorDirection = GetDamageIndicatorDirection(Origin, playerTransform);

        Vector3 indicatorPosition = new Vector3(indicatorDirection.x, indicatorDirection.y, 0) * distance;

        SetPosition(indicatorPosition);
        SetDamageIndicatorRotation(indicatorDirection);
        SetAlpha(LifeTimePercentage);
    }

    public bool IsFinished => lifeTime_left <= 0;

    public float LifeTimePercentage => lifeTime_left / lifeTime;

    public GameObject Visual => visual;


    public Vector2 GetDamageIndicatorDirection(Vector3 enemyPosition, Transform playerTransform)
    {
        // Get direction from enemy to player
        Vector3 directionToEnemy = (enemyPosition - playerTransform.position).normalized;

        // Project onto XZ plane
        Vector3 flatDirection = new Vector3(directionToEnemy.x, 0, directionToEnemy.z).normalized;

        // Convert world direction to local space relative to player's forward direction
        float forwardDot = Vector3.Dot(flatDirection, playerTransform.forward); // Front/Back
        float rightDot = Vector3.Dot(flatDirection, playerTransform.right);     // Right/Left

        // Convert to UI coordinates
        Vector2 uiDirection = new Vector2(rightDot, forwardDot).normalized;

        return uiDirection;
    }

    public void SetPosition(Vector3 position)
    {
        RectTransform rectTransform = visual.GetComponent<RectTransform>();
        rectTransform.localPosition = position;
    }

    // set rotation so that image is rotated outwards from center
    public void SetDamageIndicatorRotation(Vector2 direction)
    {
        // Convert direction to angle in degrees
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        // Rotate by 90 degrees to make it point outward (arrows usually point right at 0°)
        angle -= 90f;
        // Apply rotation
        RectTransform rectTransform = visual.GetComponent<RectTransform>();
        rectTransform.localRotation = Quaternion.Euler(0, 0, angle);
    }

    public void SetAlpha(float alpha)
    {
        RawImage image = visual.GetComponent<RawImage>();
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }

    public void DestroyVisual()
    {
        GameObject.Destroy(visual);
    }

   

}
