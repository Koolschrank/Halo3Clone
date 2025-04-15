using UnityEngine;

public class HitMarkerUI : InterfaceItem
{
    [SerializeField] GameObject hitMarker;
    [SerializeField] GameObject killMarker;
    [SerializeField] float hitMarkerTime = 0.1f;
    [SerializeField] float killMarkerTime = 0.1f;

    protected override void Subscribe(PlayerBody body)
    {
        var hitCollector = body.TargetHitCollector;
        hitCollector.OnCharacterHit += ShowHitMarker;
        hitCollector.OnCharacterKill += ShowKillMarker;
    }

    protected override void Unsubscribe(PlayerBody body)
    {
        var hitCollector = body.TargetHitCollector;
        hitCollector.OnCharacterHit -= ShowHitMarker;
        hitCollector.OnCharacterKill -= ShowKillMarker;

        HideHitMarker();
        HideKillMarker();
    }

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
