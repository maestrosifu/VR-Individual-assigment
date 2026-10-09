using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Switches the ceiling lights on and off. Because the room uses baked lighting,
/// it swaps between two baked results: one baked with the ceiling lights on and one
/// baked with them off (lightmaps for static objects, light probes for moving objects).
/// The emissive material of the light panels is swapped as well.
/// </summary>
public class RoomLightingController : MonoBehaviour
{
    private const int CoefficientsPerProbe = 27; // 3 colour channels x 9 spherical harmonics

    [SerializeField] private Renderer[] ceilingLightPanels;
    [SerializeField] private Renderer[] lampGlowEffects;
    [SerializeField] private Material lightsOnMaterial;
    [SerializeField] private Material lightsOffMaterial;

    [Header("Baked with the ceiling lights OFF")]
    [SerializeField] private Texture2D[] lightsOffLightmapColors;
    [SerializeField] private Texture2D[] lightsOffLightmapDirections;
    [SerializeField, HideInInspector] private float[] lightsOffProbeCoefficients;

    private LightmapData[] lightsOnLightmaps;
    private LightmapData[] lightsOffLightmaps;
    private SphericalHarmonicsL2[] lightsOnProbes;
    private SphericalHarmonicsL2[] lightsOffProbes;

    public bool AreLightsOn { get; private set; } = true;

    private void Awake()
    {
        lightsOnLightmaps = LightmapSettings.lightmaps;
        lightsOffLightmaps = BuildLightmapData(lightsOffLightmapColors, lightsOffLightmapDirections);

        if (LightmapSettings.lightProbes != null)
        {
            lightsOnProbes = LightmapSettings.lightProbes.bakedProbes;
            lightsOffProbes = ProbesFromCoefficients(lightsOffProbeCoefficients);
        }
    }

    public void ToggleCeilingLights()
    {
        SetCeilingLights(!AreLightsOn);
    }

    public void SetCeilingLights(bool turnOn)
    {
        AreLightsOn = turnOn;

        Material panelMaterial = turnOn ? lightsOnMaterial : lightsOffMaterial;
        foreach (Renderer panel in ceilingLightPanels)
        {
            panel.sharedMaterial = panelMaterial;
        }

        foreach (Renderer glow in lampGlowEffects)
        {
            glow.enabled = turnOn;
        }

        if (lightsOffLightmaps.Length == lightsOnLightmaps.Length && lightsOffLightmaps.Length > 0)
        {
            LightmapSettings.lightmaps = turnOn ? lightsOnLightmaps : lightsOffLightmaps;
        }

        if (lightsOnProbes != null && lightsOffProbes != null && lightsOnProbes.Length == lightsOffProbes.Length)
        {
            LightmapSettings.lightProbes.bakedProbes = turnOn ? lightsOnProbes : lightsOffProbes;
        }
    }

#if UNITY_EDITOR
    /// <summary>Editor-only: stores the "lights off" bake so it can be restored at runtime.</summary>
    public void StoreLightsOffBake(Texture2D[] colors, Texture2D[] directions, SphericalHarmonicsL2[] probes)
    {
        lightsOffLightmapColors = colors;
        lightsOffLightmapDirections = directions;
        lightsOffProbeCoefficients = CoefficientsFromProbes(probes);
    }

    private static float[] CoefficientsFromProbes(SphericalHarmonicsL2[] probes)
    {
        var coefficients = new float[probes.Length * CoefficientsPerProbe];

        for (int probe = 0; probe < probes.Length; probe++)
        {
            for (int channel = 0; channel < 3; channel++)
            {
                for (int coefficient = 0; coefficient < 9; coefficient++)
                {
                    coefficients[probe * CoefficientsPerProbe + channel * 9 + coefficient] = probes[probe][channel, coefficient];
                }
            }
        }

        return coefficients;
    }
#endif

    private static SphericalHarmonicsL2[] ProbesFromCoefficients(float[] coefficients)
    {
        if (coefficients == null || coefficients.Length == 0)
        {
            return null;
        }

        var probes = new SphericalHarmonicsL2[coefficients.Length / CoefficientsPerProbe];

        for (int probe = 0; probe < probes.Length; probe++)
        {
            for (int channel = 0; channel < 3; channel++)
            {
                for (int coefficient = 0; coefficient < 9; coefficient++)
                {
                    probes[probe][channel, coefficient] = coefficients[probe * CoefficientsPerProbe + channel * 9 + coefficient];
                }
            }
        }

        return probes;
    }

    private static LightmapData[] BuildLightmapData(Texture2D[] colors, Texture2D[] directions)
    {
        var lightmaps = new LightmapData[colors.Length];

        for (int i = 0; i < colors.Length; i++)
        {
            lightmaps[i] = new LightmapData
            {
                lightmapColor = colors[i],
                lightmapDir = i < directions.Length ? directions[i] : null
            };
        }

        return lightmaps;
    }
}
