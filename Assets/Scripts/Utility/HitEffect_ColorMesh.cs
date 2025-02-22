using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;


public class HitEffect_ColorMesh : MonoBehaviour
{
    [SerializeField] SkinnedMeshRenderer meshRenderer;
    [SerializeField] Material hitMaterial;
    [SerializeField] float hitDuration = 0.1f;
    float lastHitTime;

    Material originalMaterials;


    void Start()
    {
        originalMaterials = meshRenderer.material;
        enabled = false;
    }

    public void Hit()
    {
        
        enabled = true;
        lastHitTime = Time.time;
        meshRenderer.material = hitMaterial;
    }

    void Update()
    {
        if (lastHitTime == 0)
        {
            return;
        }

        if (Time.time - lastHitTime > hitDuration)
        {
            meshRenderer.material = originalMaterials;
            enabled = false;
        }
    }
}
