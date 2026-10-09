using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHandOnInput : MonoBehaviour
{
    [SerializeField] private InputActionProperty triggerValue;
    [SerializeField] private InputActionProperty gripValue;
    [SerializeField] private Animator handAnimator;

    private static readonly int TriggerParam = Animator.StringToHash("Trigger");
    private static readonly int GripParam = Animator.StringToHash("Grip");

    private void OnEnable()
    {
        triggerValue.action?.Enable();
        gripValue.action?.Enable();
    }

    private void Update()
    {
        if (handAnimator == null) return;

        handAnimator.SetFloat(TriggerParam, triggerValue.action.ReadValue<float>());
        handAnimator.SetFloat(GripParam, gripValue.action.ReadValue<float>());
    }
}