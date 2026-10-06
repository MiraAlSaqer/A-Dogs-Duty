using UnityEngine;

public class Gun : MonoBehaviour, IInteractable
{
    [Header("Interaction Settings")]
    [SerializeField] private string promptText = "Pick Up";

    [Header("Protect Zone Activation")]
    [SerializeField] private GameObject protectZoneObject;

    [Header("Attachment Settings")]
    [SerializeField] private Transform jawPoint;
    [SerializeField] private Vector3 holdPositionOffset = Vector3.zero;
    [SerializeField] private Vector3 holdRotationOffset = new Vector3(0f, 0f, 90f);

    [Header("Firing Parameters")]
    [SerializeField] private float roundsPerMinute = 250f;
    [SerializeField] private float projectileImpulse = 400.0f;
    [SerializeField] private float maximumDistance = 500.0f;
    [SerializeField] private LayerMask hitMask;

    [Header("Prefabs & Sockets")]
    [SerializeField] private Transform socketMuzzle;
    [SerializeField] private Transform socketEjection;
    [SerializeField] private GameObject prefabProjectile;
    [SerializeField] private GameObject prefabCasing;
    [SerializeField] private GameObject prefabMuzzleFlash;
    [SerializeField] private float muzzleFlashForwardOffset = 0.45f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private AudioClip fireSound;
    [SerializeField] private AudioClip dropSound;

    [Header("State")]
    public bool canShoot = true;
    private bool canBePickedUp = true; // Prevents picking the gun back up after round completion

    private Collider gunCollider;
    private Rigidbody gunRigidbody;
    private Camera activeCamera;
    private bool isEquipped = false;
    private float nextFireTime;
    private ithappy.Animals_FREE.MovePlayerInput playerInput;

    private void Awake()
    {
        gunCollider = GetComponent<Collider>();
        gunRigidbody = GetComponent<Rigidbody>();
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        activeCamera = Camera.main;
        protectZoneObject.SetActive(false);
    }

    private void Start()
    {
        playerInput = Object.FindFirstObjectByType<ithappy.Animals_FREE.MovePlayerInput>();
    }

    private void Update()
    {
        if (isEquipped && canShoot)
        {
            if (Input.GetMouseButton(1) || Input.GetMouseButton(0) || Input.GetButton("Fire1"))
            {
                TryShoot();
            }
        }
    }

    // Hide hover prompt if gun is equipped or permanently dropped
    public string GetPrompt()
    {
        if (isEquipped || !canBePickedUp)
        {
            return "";
        }
        else
        {
            return promptText;
        }
    }

    public void Interact()
    {
        if (isEquipped || !canBePickedUp) return;

        Transform targetSocket;
        if (jawPoint != null)
        {
            targetSocket = jawPoint;
        }
        else
        {
            targetSocket = activeCamera.transform;
        }

        audioSource.PlayOneShot(pickupSound);
        playerInput.SetCameraState(true);
        Equip(targetSocket);
        NightTaskManager.Instance.OnGunPickedUp();
    }

    public void Equip(Transform targetSocket)
    {
        isEquipped = true;

        gunRigidbody.isKinematic = true;
        gunCollider.enabled = false;

        activeCamera = Camera.main;

        transform.SetParent(targetSocket);
        transform.localPosition = holdPositionOffset;
        transform.localRotation = Quaternion.Euler(holdRotationOffset);

        protectZoneObject.SetActive(true);
    }

    private void TryShoot()
    {
        if (Time.time < nextFireTime) return;
        nextFireTime = Time.time + (60f / roundsPerMinute);
        Shoot();
    }

    private void Shoot()
    {
        audioSource.PlayOneShot(fireSound);

        Transform origin;
        if (socketMuzzle != null)
        {
            origin = socketMuzzle;
        }
        else
        {
            origin = transform;
        }

        Vector3 flashPos = origin.position + (origin.forward * muzzleFlashForwardOffset);
        GameObject flash = Instantiate(prefabMuzzleFlash, flashPos, origin.rotation);
        Destroy(flash, 0.5f);

        Instantiate(prefabCasing, socketEjection.position, socketEjection.rotation);

        Ray ray = activeCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Quaternion rotation;

        if (Physics.Raycast(ray, out RaycastHit hit, maximumDistance, hitMask))
        {
            rotation = Quaternion.LookRotation(hit.point - origin.position);
        }
        else
        {
            rotation = Quaternion.LookRotation(ray.GetPoint(maximumDistance) - origin.position);
        }

        GameObject projectile = Instantiate(prefabProjectile, origin.position, rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.linearVelocity = projectile.transform.forward * projectileImpulse;
    }

    public void Drop()
    {
        isEquipped = false;
        canShoot = false;
        canBePickedUp = false;

        transform.SetParent(null);
        transform.position += Vector3.up * 0.25f;
        gunCollider.isTrigger = false;
        gunCollider.enabled = true;

        gunRigidbody.isKinematic = false;
        gunRigidbody.useGravity = true;
        gunRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        gunRigidbody.linearVelocity = Vector3.zero;
        gunRigidbody.angularVelocity = Vector3.zero;
        gunRigidbody.AddForce(transform.forward * 0.4f + Vector3.down * 0.2f, ForceMode.Impulse);

        audioSource.PlayOneShot(dropSound);
    }
}