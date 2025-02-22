using UnityEngine;

public class BulletTrail : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] float trailDuration = 0.1f;

    float trailTimer;

    public void ShowTrail( Vector3 end)
    {
        gameObject.SetActive(true);
        lineRenderer.SetPosition(1, end );
        trailTimer = trailDuration;
    }

    private void Update()
    {
        if (trailTimer > 0)
        {
            trailTimer -= Time.deltaTime;
            if (trailTimer <= 0)
            {
                lineRenderer.SetPosition(0, Vector3.zero);
                lineRenderer.SetPosition(1, Vector3.zero);
                gameObject.SetActive(false);
                Destroy(gameObject);
            }
        }
    }
}
