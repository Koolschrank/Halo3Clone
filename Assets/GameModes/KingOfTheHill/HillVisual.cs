using UnityEngine;

public class HillVisual : MonoBehaviour
{
    [SerializeField] Hill hill;
    [SerializeField] MeshRenderer meshRenderer;

    [SerializeField] Material baseMaterial;
    [SerializeField] Material[] teamMaterials;


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

    public void OnTeamChange(int team)
    {
        // if team index is out of bounds, use the base material
        if (team < 0 || team >= teamMaterials.Length)
        {
            meshRenderer.material = baseMaterial;
            return;
        }

        meshRenderer.material = teamMaterials[team];

    }
}
