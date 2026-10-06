using UnityEngine;

public class FoodSack : MonoBehaviour
{
    [SerializeField] private GameObject fullSack;
    [SerializeField] private GameObject emptySack;

    private void Start()
    {
        fullSack.SetActive(true);
        emptySack.SetActive(false);
    }

    public void EmptySack()
    {
        fullSack.SetActive(false);
        emptySack.SetActive(true);

        Destroy(GetComponent<Rigidbody>());

        Collider[] childColliders = GetComponentsInChildren<Collider>();
        foreach (Collider childCollider in childColliders)
        {
            Destroy(childCollider);
        }
    }
}