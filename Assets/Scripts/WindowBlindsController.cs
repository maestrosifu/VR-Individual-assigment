using System.Collections;
using UnityEngine;

/// <summary>
/// Rolls the window blinds up or down and dims the sunlight coming into the room.
/// </summary>
public class WindowBlindsController : MonoBehaviour
{
    [Tooltip("Blind objects whose pivot is at the top, so scaling Y rolls them down.")]
    [SerializeField] private Transform[] blinds;
    [SerializeField] private Light sunLight;
    [SerializeField] private float openedHeightScale = 0.15f;
    [SerializeField] private float closedHeightScale = 1f;
    [SerializeField] private float openedSunIntensity = 1.5f;
    [SerializeField] private float closedSunIntensity = 0.3f;
    [SerializeField] private float animationDuration = 1.5f;
    [SerializeField] private bool startClosed;

    private Coroutine blindsRoutine;

    public bool AreClosed { get; private set; }

    private void Start()
    {
        AreClosed = startClosed;
        ApplyBlindState(AreClosed ? 1f : 0f);
    }

    public void ToggleBlinds()
    {
        AreClosed = !AreClosed;

        if (blindsRoutine != null)
        {
            StopCoroutine(blindsRoutine);
        }

        blindsRoutine = StartCoroutine(AnimateBlinds(AreClosed ? 1f : 0f));
    }

    private IEnumerator AnimateBlinds(float targetClosedAmount)
    {
        float startClosedAmount = Mathf.InverseLerp(openedHeightScale, closedHeightScale, blinds[0].localScale.y);

        for (float elapsed = 0f; elapsed < animationDuration; elapsed += Time.deltaTime)
        {
            float t = Mathf.SmoothStep(0f, 1f, elapsed / animationDuration);
            ApplyBlindState(Mathf.Lerp(startClosedAmount, targetClosedAmount, t));
            yield return null;
        }

        ApplyBlindState(targetClosedAmount);
        blindsRoutine = null;
    }

    private void ApplyBlindState(float closedAmount)
    {
        float heightScale = Mathf.Lerp(openedHeightScale, closedHeightScale, closedAmount);

        foreach (Transform blind in blinds)
        {
            Vector3 scale = blind.localScale;
            blind.localScale = new Vector3(scale.x, heightScale, scale.z);
        }

        if (sunLight != null)
        {
            sunLight.intensity = Mathf.Lerp(openedSunIntensity, closedSunIntensity, closedAmount);
        }
    }
}
