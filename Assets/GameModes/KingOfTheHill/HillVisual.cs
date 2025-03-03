using UnityEngine;

public class HillVisual : MonoBehaviour
{
    [SerializeField] Hill hill;

    [SerializeField] GameObject[] teamAuras;


    public void Awake()
    {
        hill.OnTeamChanged += OnTeamChange;
        hill.OnActivated += OnActivated;
        hill.OnDeactivated += OnDeactivated;
        transform.localScale = Vector3.one * hill.Radius * 2;
        OnTeamChange(-1);



    }

    private void OnActivated()
    {
        OnTeamChange(-1);
    }

    private void OnDeactivated()
    {
        teamAuras[0].SetActive(false);
        teamAuras[1].SetActive(false);
        teamAuras[1].SetActive(false);
    }

    // activate and deactivate the hill


    public void OnTeamChange(int team)
    {
        // if team index is out of bounds, use the base material
        if (team < 0)
        {
            teamAuras[0].SetActive(true);
            teamAuras[1].SetActive(false);
            teamAuras[2].SetActive(false);
            return;
        }
        else if (team == 0)
        {
            teamAuras[0].SetActive(false);
            teamAuras[1].SetActive(true);
            teamAuras[2].SetActive(false);
        }
        else {
            teamAuras[0].SetActive(false);
            teamAuras[1].SetActive(false);
            teamAuras[2].SetActive(true);
        }

    }
}
