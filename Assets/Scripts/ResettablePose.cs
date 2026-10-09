using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// Remembers where a physics object started so it can be smoothly moved back there later.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class ResettablePose : MonoBehaviour
{
    private Rigidbody body;
    private XRGrabInteractable grabInteractable;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private bool startsKinematic;
    private Coroutine resetRoutine;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();
        startPosition = transform.position;
        startRotation = transform.rotation;
        startsKinematic = body.isKinematic;
    }

    public void ResetToStart(float duration)
    {
        if (grabInteractable != null && grabInteractable.isSelected)
        {
            grabInteractable.interactionManager.CancelInteractableSelection((IXRSelectInteractable)grabInteractable);
        }

        if (resetRoutine != null)
        {
            StopCoroutine(resetRoutine);
        }

        resetRoutine = StartCoroutine(MoveBackToStart(duration));
    }

    private IEnumerator MoveBackToStart(float duration)
    {
        body.isKinematic = true;

        Vector3 fromPosition = transform.position;
        Quaternion fromRotation = transform.rotation;

        for (float elapsed = 0f; elapsed < duration; elapsed += Time.deltaTime)
        {
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            // Small arc so the piece visibly "flies" back to its square.
            Vector3 arc = Vector3.up * (Mathf.Sin(t * Mathf.PI) * 0.4f);
            transform.SetPositionAndRotation(
                Vector3.Lerp(fromPosition, startPosition, t) + arc,
                Quaternion.Slerp(fromRotation, startRotation, t));
            yield return null;
        }

        transform.SetPositionAndRotation(startPosition, startRotation);
        body.isKinematic = startsKinematic;
        if (!body.isKinematic)
        {
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }

        resetRoutine = null;
    }
}
