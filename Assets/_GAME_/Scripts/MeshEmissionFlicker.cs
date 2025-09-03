using System.Collections;
using UnityEngine;

public class MeshEmissionFlicker : MonoBehaviour
{
    [Header("Flicker Settings")]
    public int flickerCount = 3;
    public float flickerInterval = 0.2f;
    [Tooltip("How bright the white emission will be (HDR intensity)")]
    public float emissionIntensity = 3f;

    private MeshRenderer[] meshRenderers;

    void Awake()
    {
        // Cache all child mesh renderers
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
    }

    public void StartEmissionFlicker()
    {
        StopAllCoroutines();
        StartCoroutine(FlickerEmissionRoutine());
    }

    private IEnumerator FlickerEmissionRoutine()
    {
        // Make sure emission keyword is enabled
        foreach (var renderer in meshRenderers)
        {
            foreach (var mat in renderer.materials)
            {
                mat.EnableKeyword("_EMISSION");
            }
        }

        for (int i = 0; i < flickerCount; i++)
        {
            // Turn emission white with intensity
            SetEmissionColor(Color.white * emissionIntensity);
            yield return new WaitForSeconds(flickerInterval);

            // Turn emission off (black)
            SetEmissionColor(Color.black);
            yield return new WaitForSeconds(flickerInterval);
        }

        // Ensure it ends fully black
        SetEmissionColor(Color.black);
    }

    private void SetEmissionColor(Color color)
    {
        foreach (var renderer in meshRenderers)
        {
            foreach (var mat in renderer.materials)
            {
                mat.SetColor("_EmissionColor", color);
            }
        }
    }
}
