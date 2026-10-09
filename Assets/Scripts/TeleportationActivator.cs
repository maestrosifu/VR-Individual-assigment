using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class TeleportationActivator : MonoBehaviour
{
    [SerializeField] private XRRayInteractor teleportInteractor;
    [SerializeField] private InputActionProperty teleportActivatorAction;

    private void Start()
    {
        teleportInteractor.gameObject.SetActive(false);
        teleportActivatorAction.action.performed += Action_performed;
    }

    private void OnDestroy()
    {
        if (teleportActivatorAction.action != null)
        {
            teleportActivatorAction.action.performed -= Action_performed;
        }
    }

    private void Action_performed(InputAction.CallbackContext obj)
    {
        teleportInteractor.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (teleportActivatorAction.action.WasReleasedThisFrame())
        {
            teleportInteractor.gameObject.SetActive(false);
        }
    }
}