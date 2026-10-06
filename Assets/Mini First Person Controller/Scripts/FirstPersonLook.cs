using UnityEngine;

public class FirstPersonLook : MonoBehaviour
{
    [SerializeField] private Transform character;
    public float sensitivity = 2f;
    public float smoothing = 1.5f;
    public bool invertY = false;

    public bool canLook = true;

    private Vector2 velocity;
    private Vector2 frameVelocity;

    void OnEnable()
    {
        SettingsManager.OnSensitivityChanged += ApplySensitivity;
        SettingsManager.OnInvertLookChanged += ApplyInvertLook;

        sensitivity = PlayerPrefs.GetFloat("Sensitivity", 2f);
        invertY = PlayerPrefs.GetInt("InvertLook", 0) == 1;

        if (character != null)
        {
            velocity.x = character.localEulerAngles.y;
            velocity.y = 0;
        }
    }

    void OnDisable()
    {
        SettingsManager.OnSensitivityChanged -= ApplySensitivity;
        SettingsManager.OnInvertLookChanged -= ApplyInvertLook;
    }

    private void ApplySensitivity(float newSensitivity)
    {
        sensitivity = newSensitivity;
    }

    private void ApplyInvertLook(bool invert)
    {
        invertY = invert;
    }

    void Update()
    {
        if (!canLook) return;

        float yMultiplier = invertY ? -1f : 1f;

        Vector2 mouseDelta = new Vector2(
            Input.GetAxisRaw("Mouse X"),
            Input.GetAxisRaw("Mouse Y") * yMultiplier
        );

        Vector2 rawFrameVelocity = Vector2.Scale(mouseDelta, Vector2.one * sensitivity);
        frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, 1 / smoothing);
        velocity += frameVelocity;

        velocity.y = Mathf.Clamp(velocity.y, -90, 90);
    }

    void LateUpdate()
    {
        if (!canLook) return;

        transform.localRotation = Quaternion.AngleAxis(-velocity.y, Vector3.right);

        if (character != null)
        {
            character.localRotation = Quaternion.AngleAxis(velocity.x, Vector3.up);
        }
    }
}