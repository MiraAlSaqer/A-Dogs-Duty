using UnityEngine;
using TMPro;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float interactRange = 2f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("UI Settings")]
    [SerializeField] private GameObject UIGroup;
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private Vector3 promptOffset = new Vector3(0, 0, 0);

    private IInteractable currentTarget;
    private Collider targetCollider;

    private void Start()
    {
        UIGroup.SetActive(false);
    }

    private void Update()
    {
        FindInteractable();
        UpdateUI();

        if (Input.GetKeyDown(interactKey))
        {
            if (currentTarget != null)
            {
                currentTarget.Interact();
            }
            else
            {
                DogMouth dogMouth = GetComponent<DogMouth>();
                if (dogMouth != null && dogMouth.heldObject != null)
                {
                    Bucket heldBucket = dogMouth.heldObject.GetComponent<Bucket>();
                    heldBucket.Drop();
                }
            }
        }
    }

    private void FindInteractable()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactRange, interactableLayer);

        IInteractable closest = null;
        Collider closestCol = null;
        float closestDist = Mathf.Infinity;

        foreach (var hit in hits)
        {
            IInteractable interactable = hit.GetComponentInParent<IInteractable>();

            if (interactable != null && interactable.GetPrompt() != "")
            {
                // Measure distance to the physical center of the box
                float dist = Vector3.Distance(transform.position, hit.bounds.center);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = interactable;
                    closestCol = hit;
                }
            }
        }

        currentTarget = closest;
        targetCollider = closestCol;
    }

    private void UpdateUI()
    {
        if (currentTarget != null && UIGroup != null && promptText != null && currentTarget.GetPrompt() != "")
        {
            UIGroup.SetActive(true);
            promptText.text = currentTarget.GetPrompt();

            Camera activeCam = Camera.main;
            if (activeCam != null)
            {
                // Lock UI to the exact center of the collider of the object
                Vector3 screenPos = activeCam.WorldToScreenPoint(targetCollider.bounds.center + promptOffset);
                UIGroup.transform.position = screenPos;
            }
        }
        else if (UIGroup != null)
        {
            UIGroup.SetActive(false);
        }
    }
}