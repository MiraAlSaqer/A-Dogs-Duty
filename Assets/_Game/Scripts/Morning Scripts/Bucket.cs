using UnityEngine;

public class Bucket : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject water;

    [Header("Grab Transform")]
    [SerializeField] private Vector3 holdPosition = new Vector3(0f, 0.09f, -0.411f);
    [SerializeField] private Vector3 holdRotation = new Vector3(100f, 0f, 0f);

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip pickDropSound;
    public AudioClip fillSound;

    [HideInInspector] public bool hasWater;
    
    private Rigidbody rb;
    private DogMouth dog;
    private bool isHeld;
    private int originalLayer;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        dog = Object.FindFirstObjectByType<DogMouth>();
        originalLayer = gameObject.layer;
        water.SetActive(false);
    }

    public string GetPrompt()
    {
        if (isHeld)
        {
            return "";
        }
        else
        {
            return "Pick Up";
        }
    }

    public void Interact()
    {
        // Only allow picking up if currently free and the dog's mouth is empty
        if (isHeld || dog.heldObject != null) return;

        isHeld = true;
        dog.heldObject = gameObject;
        audioSource.PlayOneShot(pickDropSound);

        transform.SetParent(dog.jaw);
        transform.localPosition = holdPosition;
        transform.localEulerAngles = holdRotation;

        rb.isKinematic = true;
        gameObject.layer = LayerMask.NameToLayer("Default");
    }

    public void Fill()
    {
        hasWater = true;
        water.SetActive(true);
        audioSource.PlayOneShot(fillSound);
    }

    public void EmptyWater()
    {
        hasWater = false;
        water.SetActive(false);
    }

    public void Drop()
    {
        EmptyWater();
        isHeld = false;
        dog.heldObject = null;
        
        transform.SetParent(null);
        rb.isKinematic = false;
        gameObject.layer = originalLayer;
        
        audioSource.PlayOneShot(pickDropSound);
    }
}