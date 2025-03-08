using System.Drawing;
using UnityEngine;

public class HillVisual : MonoBehaviour
{
    [SerializeField] Hill hill;


    [Header("Visual")]
    [SerializeField] ParticleSystem particalSystem;
    [SerializeField] UnityEngine.Color baseColor;
    [SerializeField] UnityEngine.Color[] teamColors;


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
        particalSystem.gameObject.SetActive(true);
    }

    private void OnDeactivated()
    {
        particalSystem.gameObject.SetActive(false);
    }

    // activate and deactivate the hill


    public void OnTeamChange(int team)
    {
        var color = team < 0 ? baseColor : teamColors[team];
        
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particalSystem.particleCount];
        int numParticlesAlive = particalSystem.GetParticles(particles); // Get all active particles

        for (int i = 0; i < numParticlesAlive; i++)
        {
            particles[i].startColor = color; // Change active particle color
        }

        particalSystem.SetParticles(particles, numParticlesAlive);

        var main = particalSystem.main;
        main.startColor = color;


        

    }
}
