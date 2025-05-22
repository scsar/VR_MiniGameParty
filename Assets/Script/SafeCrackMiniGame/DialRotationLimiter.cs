using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DialRotationLimiter : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;
    private bool isGrabbed = false;
    private Quaternion initialGrabRotation;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        rb.isKinematic = true;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed = false;
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.angularVelocity = Vector3.zero;
            rb.linearVelocity = Vector3.zero;
        }

        Vector3 currentRotation = transform.localEulerAngles;
        transform.localEulerAngles = new Vector3(currentRotation.x, -90f, -90f);
    }

    private void FixedUpdate()
    {
        if (!isGrabbed)
            return;

        // 현재 회전 값을 가져오고
        Vector3 currentEuler = transform.localEulerAngles;

        transform.localEulerAngles = new Vector3(currentEuler.x, -90f, -90f);
    }
}
