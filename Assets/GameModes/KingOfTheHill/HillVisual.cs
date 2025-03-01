using UnityEngine;

public class HillVisual : MonoBehaviour
{
    [SerializeField] Hill hill;
    [SerializeField] MeshRenderer meshRenderer;

    [SerializeField] Material baseMaterial;
    [SerializeField] Material blueMaterial;
    [SerializeField] Material redMaterial;

    public void Awake()
    {
        hill.OnTeamChanged += OnTeamChange;
        hill.OnActivated += OnActivated;
        hill.OnDeactivated += OnDeactivated;
        transform.localScale = Vector3.one * hill.Radius * 2;
        

    }

    // activate and deactivate the hill
    public void OnActivated()
    {
        meshRenderer.enabled = true;
    }

    public void OnDeactivated()
    {
        meshRenderer.enabled = false;
    }

    public void OnTeamChange(Team team)
    {
        switch (team)
        {
            case Team.None:
                meshRenderer.material = baseMaterial;
                break;
            case Team.Blue:
                meshRenderer.material = blueMaterial;
                break;
            case Team.Red:
                meshRenderer.material = redMaterial;
                break;
        }
    }
}
