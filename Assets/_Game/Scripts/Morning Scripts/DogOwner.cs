using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using ithappy.Animals_FREE;

public class DogOwner : MonoBehaviour, IInteractable
{
    [Header("Audio")]
    public AudioClip barkSound;
    public AudioSource dogAudioSource;

    [Header("Dialogue")]
    [TextArea(2, 4)]
    public string[] farmerDialogue = {
        "Mornin', boy.",
        "You ready to earn your keep?",
        "We got a long day ahead of us...",
        "So shake off the sleep and let's get to work."
    };

    [Header("Morning Sequence Transition")]
    [SerializeField] private MovePlayerInput dogMovement;
    [SerializeField] private Transform dogRootTransform;
    [SerializeField] private Image fadeScreen;
    [SerializeField] private GameObject houseBounds;
    [SerializeField] private float fadeDuration = 1.5f;

    private bool hasInteracted;

    public string GetPrompt()
    {
        if (hasInteracted)
        {
            return "";
        }
        else
        {
            return "Interact";
        }
    }

    public void Interact()
    {
        if (hasInteracted) return;

        hasInteracted = true;
        DialogueManager.Instance.StartDialogue(farmerDialogue);
        StartCoroutine(WaitForDialogueAndSequence());
    }

    private IEnumerator WaitForDialogueAndSequence()
    {
        yield return null;

        while (DialogueManager.Instance.dialogueBox.activeInHierarchy)
        {
            yield return null;
        }

        dogAudioSource.PlayOneShot(barkSound);
        yield return new WaitForSeconds(1.0f);

        fadeScreen.gameObject.SetActive(true);
        fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, 0f);
        yield return fadeScreen.DOFade(1f, fadeDuration).WaitForCompletion();

        dogMovement.SetCameraState(false);
        houseBounds.SetActive(false);

        Vector3 rot = dogRootTransform.eulerAngles;
        rot.x = 0f;
        dogRootTransform.eulerAngles = rot;

        yield return fadeScreen.DOFade(0f, fadeDuration).WaitForCompletion();
        fadeScreen.gameObject.SetActive(false);

        TaskManager.Instance.InitializeTasks();
    }
}