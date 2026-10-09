using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// When a chess piece is released above the board, it stands up straight in the centre
/// of the nearest square. When it is released anywhere else it simply falls with physics.
/// </summary>
[RequireComponent(typeof(XRGrabInteractable), typeof(Rigidbody))]
public class ChessPieceSnapper : MonoBehaviour
{
    [SerializeField] private float snapDuration = 0.15f;

    private XRGrabInteractable grabInteractable;
    private Rigidbody body;
    private ChessBoardGrid board;
    private Coroutine snapRoutine;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        body = GetComponent<Rigidbody>();
        board = GetComponentInParent<ChessBoardGrid>();
    }

    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (snapRoutine != null)
        {
            StopCoroutine(snapRoutine);
            snapRoutine = null;
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        if (board != null && board.TryGetSquareCenter(transform.position, out Vector3 squareCenter))
        {
            snapRoutine = StartCoroutine(StandOnSquare(squareCenter));
            return;
        }

        body.isKinematic = false;
    }

    private IEnumerator StandOnSquare(Vector3 squareCenter)
    {
        body.isKinematic = true;

        Vector3 fromPosition = transform.position;
        Quaternion fromRotation = transform.rotation;
        Quaternion uprightRotation = GetUprightRotation();

        for (float elapsed = 0f; elapsed < snapDuration; elapsed += Time.deltaTime)
        {
            float t = Mathf.SmoothStep(0f, 1f, elapsed / snapDuration);
            transform.SetPositionAndRotation(
                Vector3.Lerp(fromPosition, squareCenter, t),
                Quaternion.Slerp(fromRotation, uprightRotation, t));
            yield return null;
        }

        transform.SetPositionAndRotation(squareCenter, uprightRotation);
        snapRoutine = null;
    }

    private Quaternion GetUprightRotation()
    {
        // Keep the direction the piece is facing (important for knights), but remove any tilt.
        Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
        if (flatForward.sqrMagnitude < 0.01f)
        {
            flatForward = Vector3.ProjectOnPlane(transform.up, Vector3.up);
        }

        return flatForward.sqrMagnitude < 0.01f ? Quaternion.identity : Quaternion.LookRotation(flatForward, Vector3.up);
    }
}
