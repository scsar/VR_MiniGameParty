using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class WireCutter : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;

    public InputActionReference leftTriggerButton;
    public InputActionReference rightTriggerButton;

    public Animation cutAni;

    private bool isHeld = false;
    private bool isTriggerPressed = false;
    private bool isSubscribed = false;

    public float rayDistance = 0.2f; // 자를 수 있는 거리
    public LayerMask wireLayer;      // Wire만 맞추기 위한 레이어 설정

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        isHeld = true;

        if (!isSubscribed)
        {
            if (leftTriggerButton?.action != null)
            {
                leftTriggerButton.action.started += OnTriggerPressed;
                leftTriggerButton.action.canceled += OnTriggerReleased;
            }

            if (rightTriggerButton?.action != null)
            {
                rightTriggerButton.action.started += OnTriggerPressed;
                rightTriggerButton.action.canceled += OnTriggerReleased;
            }

            isSubscribed = true;
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isHeld = false;

        if (isSubscribed)
        {
            if (leftTriggerButton?.action != null)
            {
                leftTriggerButton.action.started -= OnTriggerPressed;
                leftTriggerButton.action.canceled -= OnTriggerReleased;
            }

            if (rightTriggerButton?.action != null)
            {
                rightTriggerButton.action.started -= OnTriggerPressed;
                rightTriggerButton.action.canceled -= OnTriggerReleased;
            }

            isSubscribed = false;
        }
    }

    private void OnTriggerPressed(InputAction.CallbackContext context)
    {
        if (!isHeld) return; // 반드시 Grab 중일 때만
        isTriggerPressed = true;
    }

    private void OnTriggerReleased(InputAction.CallbackContext context)
    {
        if (!isHeld) return;
        isTriggerPressed = false;
    }

    void Update()
    {
        if (isHeld && isTriggerPressed)
        {
            TryCutWire();
        }
    }

    private void TryCutWire()
    {
        cutAni.Play();
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, wireLayer))
        {
            if (hit.collider != null)
            {
                Debug.Log("[WireCutter] Wire hit: " + hit.collider.name);

                WireInteraction wire = hit.collider.GetComponent<WireInteraction>();
                if (wire != null)
                {
                    wire.OnCut();
                }
            }
        }
    }
}