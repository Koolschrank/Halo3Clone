using UnityEngine;

public class HitMarkerUI : MonoBehaviour
{
    [SerializeField] GameObject hitMarker;
    [SerializeField] GameObject killMarker;
    [SerializeField] float hitMarkerTime = 0.1f;
    [SerializeField] float killMarkerTime = 0.1f;

    public void ShowHitMarker(GameObject target)
    {
        hitMarker.SetActive(true);
        Invoke("HideHitMarker", hitMarkerTime);
    }

    public void ShowKillMarker(GameObject target)
    {
        killMarker.SetActive(true);
        Invoke("HideKillMarker", killMarkerTime);
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
