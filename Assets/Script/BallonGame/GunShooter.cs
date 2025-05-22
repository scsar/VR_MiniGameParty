using UnityEngine;
using UnityEngine.InputSystem;

public class GunShooter : MonoBehaviour
{
    public InputActionReference triggerButton; // 🔥 트리거 버튼 (Input Action Reference)
    public float shootDistance = 20f;           // 사정거리
    public LayerMask balloonLayerMask;          // 풍선만 맞추기 위한 레이어 필터

    private void OnEnable()
    {
        if (triggerButton != null)
        {
            triggerButton.action.performed += OnTriggerPressed;
        }
    }

    private void OnTriggerPressed(InputAction.CallbackContext context)
    {
        Debug.Log("Trigger");
        Shoot();
    }

    private void Shoot()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * shootDistance, Color.red, 2.0f);
        if (Physics.Raycast(ray, out RaycastHit hit, shootDistance, balloonLayerMask))
        {
            Balloon balloon = hit.collider.GetComponent<Balloon>();
            if (balloon != null)
            {
                balloon.OnPop();
            }
        }
    }
}
