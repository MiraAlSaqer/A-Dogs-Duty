using UnityEngine;

public class PushObject : MonoBehaviour
{
    [SerializeField] private float pushPower = 2.0f;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rb = hit.collider.attachedRigidbody;

        if (rb == null || rb.isKinematic) return;
        if (hit.moveDirection.y < -0.3f) return;

        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        rb.AddForce(pushDir * pushPower, ForceMode.VelocityChange);
    }
}