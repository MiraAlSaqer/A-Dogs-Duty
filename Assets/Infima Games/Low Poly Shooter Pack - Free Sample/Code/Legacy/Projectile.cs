using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float lifetime = 4.0f;

    [Header("Impact Effects")]
    [Tooltip("Drag your blood explosion/splatter particle prefab here.")]
    [SerializeField] private GameObject bloodImpactPrefab;
    [Tooltip("Optional ground/dust impact prefab for misses.")]
    [SerializeField] private GameObject defaultImpactPrefab;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandleHit(collision.gameObject, collision.contacts[0].point, collision.contacts[0].normal);
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleHit(other.gameObject, transform.position, -transform.forward);
    }

    private void HandleHit(GameObject hitObj, Vector3 hitPoint, Vector3 normal)
    {
        // Ignore the player dog or other active bullets
        if (hitObj.CompareTag("Player") || hitObj.GetComponent<Projectile>() != null)
            return;

        Wolf wolf = hitObj.GetComponentInParent<Wolf>();

        // Check if we hit a wolf
        if (wolf != null || hitObj.CompareTag("Wolf"))
        {
            if (wolf != null)
            {
                wolf.TakeDamage();
            }

            // Spawn blood effect directly facing out from the impact point
            if (bloodImpactPrefab != null)
            {
                GameObject blood = Instantiate(bloodImpactPrefab, hitPoint, Quaternion.LookRotation(normal));
                Destroy(blood, 2.5f);
            }

            Destroy(gameObject);
            return;
        }

        // Hit terrain/fences (non-wolf)
        if (defaultImpactPrefab != null)
        {
            GameObject dirt = Instantiate(defaultImpactPrefab, hitPoint, Quaternion.LookRotation(normal));
            Destroy(dirt, 2.5f);
        }

        Destroy(gameObject);
    }
}