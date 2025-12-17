using UnityEngine;
using UnityEngine.UI;

public class HitMarkerUI : MonoBehaviour
{
    [SerializeField] bool showHitMarker = true;
    [SerializeField] bool showKillMarker = true;
    [SerializeField] GameObject hitMarker;
    [SerializeField] GameObject killMarker;
    [SerializeField] RawImage awsomeSkull;
    [SerializeField] float hitMarkerTime = 0.1f;
    [SerializeField] float killMarkerTime = 0.1f;
    [SerializeField] float awsomeSkullTime = 0.5f;
    float awsomeSkullTimer = 0;
    [SerializeField] AnimationCurve awsomeSkullFadeCurve;

    public float killMarkerScale;
    public AnimationCurve killMarkerScaleCurve;
    public AnimationCurve killMarkerFadeCurve;

    RawImage killImiage;

	private void Awake()
	{
		killImiage = killMarker.GetComponent<RawImage>();
	}

	public void ShowHitMarker(GameObject target)
    {
        if (!showHitMarker)
            return;
        if (target.tag == "AIEnemy")
            return;


        hitMarker.SetActive(true);
        Invoke("HideHitMarker", hitMarkerTime);
    }

    float killMarkerStartTime;
    bool killMarkerActive = false;

	public void ShowKillMarker(GameObject target)
    {
        if (!showKillMarker)
            return;
        if (target.tag == "AIEnemy")
        {
			hitMarker.SetActive(true);
			Invoke("HideHitMarker", hitMarkerTime);
            return;
		}

        killMarker.SetActive(true);
        awsomeSkull.gameObject.SetActive(true);
        awsomeSkullTimer = awsomeSkullTime;
        killMarkerStartTime = Time.time;
        killMarkerActive = true;
	}

    private void Update()
    {
        if (awsomeSkullTimer > 0)
        {
            awsomeSkullTimer -= Time.deltaTime;
            float t = Mathf.Clamp01(awsomeSkullTimer / awsomeSkullTime);
            float alpha = awsomeSkullFadeCurve.Evaluate(1-t);
            Color color = awsomeSkull.color;
            color.a = alpha;
            awsomeSkull.color = color;

            if (awsomeSkullTimer <= 0)
            {
                awsomeSkull.gameObject.SetActive(false);
            }
        }

        if (killMarkerActive)
        {
            float t = Time.time - killMarkerStartTime;
            float percent = t / killMarkerTime;
			float scale = killMarkerScaleCurve.Evaluate(percent);
            Color color = killImiage.color;
            color.a = killMarkerFadeCurve.Evaluate(percent);
			killImiage.color = color;
            killMarker.transform.localScale = Vector3.one * scale * killMarkerScale;
            
			// Hide kill marker after time ove
            if (t > killMarkerTime)
            {
                HideKillMarker();
			}
		}
	}

    void HideHitMarker()
    {
        hitMarker.SetActive(false);
    }

    void HideKillMarker()
    {
        killMarker.SetActive(false);

    }



}
