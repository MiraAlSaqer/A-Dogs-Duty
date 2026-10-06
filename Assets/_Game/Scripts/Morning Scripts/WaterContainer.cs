using UnityEngine;

public class WaterContainer : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject water;
    private DogMouth dog;
    private bool isFull = false;
    public AudioSource audioSource;
    public AudioClip emptySound;

    private void Start()
    {
        dog = Object.FindFirstObjectByType<DogMouth>();
        water.SetActive(false);
    }

    public string GetPrompt()
    {
        if (isFull) return "";

        if (dog.heldObject != null)
        {
            Bucket bucket = dog.heldObject.GetComponent<Bucket>();
            if (bucket.hasWater)
            {
                return "Fill Container";
            }
        }
        return "";
    }

    public void Interact()
    {
        if (!isFull && dog.heldObject != null)
        {
            Bucket bucket = dog.heldObject.GetComponent<Bucket>();
            if (bucket.hasWater)
            {
                bucket.EmptyWater();
                water.SetActive(true);
                isFull = true;
                audioSource.PlayOneShot(emptySound);

                TaskManager.Instance.CompleteCurrentTask();
            }
        }
    }
}