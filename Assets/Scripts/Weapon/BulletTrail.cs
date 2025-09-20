using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class BulletTrail : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] float trailDuration = 0.1f;
    public bool trailFadeOut;
    float trailTimer;
    float initialWidth;
	public void ShowTrail( Vector3 end)
    {
        gameObject.SetActive(true);
        lineRenderer.SetPosition(1, end );
        trailTimer = trailDuration;
        initialWidth = lineRenderer.startWidth;
	}

    public void ShowTrail(Vector3[] points)
    {
        gameObject.SetActive(true);
        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);
        trailTimer = trailDuration;
        initialWidth = lineRenderer.startWidth;
	}

    private void Update()
    {
        if (trailTimer > 0)
        {
            trailTimer -= Time.deltaTime;
            if (trailFadeOut)
            {
                var width = Mathf.Lerp(0, initialWidth, trailTimer / trailDuration);
                lineRenderer.startWidth = width;
                lineRenderer.endWidth = width;
			}

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
