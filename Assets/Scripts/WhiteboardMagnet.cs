using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// A magnetic card that sticks flat onto the whiteboard when it is released close to it,
/// and falls to the floor when it is released anywhere else.
/// </summary>
[RequireComponent(typeof(XRGrabInteractable), typeof(Rigidbody))]
public class WhiteboardMagnet : MonoBehaviour
{
    [Tooltip("Surface the magnet sticks to. Its forward axis must point into the room.")]
    [SerializeField] private Transform whiteboardSurface;
    [SerializeField] private float snapDistance = 0.15f;
    [SerializeField] private float halfThickness = 0.004f;

    private XRGrabInteractable grabInteractable;
    private Rigidbody body;

    public void SetWhiteboard(Transform surface)
    {
        whiteboardSurface = surface;
    }

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        body = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    private void OnDisable()
    {
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        if (TryStickToWhiteboard())
        {
            return;
        }

        body.isKinematic = false;
    }

    private bool TryStickToWhiteboard()
    {
        if (whiteboardSurface == null)
        {
            return false;
        }

        Vector3 boardNormal = whiteboardSurface.forward;
        var boardPlane = new Plane(boardNormal, whiteboardSurface.position);
        float distanceToBoard = boardPlane.GetDistanceToPoint(transform.position);

        if (Mathf.Abs(distanceToBoard) > snapDistance)
        {
            return false;
        }

        // Keep the player's roll, but lay the card flat against the board.
        Vector3 upOnBoard = Vector3.ProjectOnPlane(transform.up, boardNormal);
        if (upOnBoard.sqrMagnitude < 0.001f)
        {
            upOnBoard = Vector3.up;
        }

        Vector3 snappedPosition = boardPlane.ClosestPointOnPlane(transform.position) + boardNormal * halfThickness;
        body.isKinematic = true;
        transform.SetPositionAndRotation(snappedPosition, Quaternion.LookRotation(-boardNormal, upOnBoard));
        return true;
    }
}
