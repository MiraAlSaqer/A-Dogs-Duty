using UnityEngine;

public class WaterWell : MonoBehaviour, IInteractable
{
    private DogMouth dog;

    private void Start()
    {
        dog = Object.FindFirstObjectByType<DogMouth>();
    }

    public string GetPrompt()
    {
        if (dog.heldObject != null)
        {
            Bucket bucket = dog.heldObject.GetComponent<Bucket>();
            if (!bucket.hasWater)
            {
                return "Fill Bucket";
            }
        }
        return "";
    }

    public void Interact()
    {
        if (dog.heldObject != null)
        {
            Bucket bucket = dog.heldObject.GetComponent<Bucket>();
            if (!bucket.hasWater)
            {
                bucket.Fill();
            }
        }
    }
}