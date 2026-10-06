using UnityEngine;

namespace ithappy.Animals_FREE
{
    [RequireComponent(typeof(CreatureMover))]
    public class MovePlayerInput : MonoBehaviour
    {
        public enum StartingCameraMode
        {
            FirstPerson,
            ThirdPerson,
            LeaveForCutscene
        }

        [Header("Initial Setup")]
        [SerializeField] private StartingCameraMode startingCamera = StartingCameraMode.LeaveForCutscene;

        [Header("Audio")]
        [SerializeField] private AudioSource footstepSource;
        [SerializeField] private AudioSource barkSource;
        [SerializeField] private AudioClip runLoop;
        [SerializeField] private AudioClip barkSound;

        [Header("Character")]
        [SerializeField] private string m_HorizontalAxis = "Horizontal";
        [SerializeField] private string m_VerticalAxis = "Vertical";
        [SerializeField] private string m_JumpButton = "Jump";
        [SerializeField] private KeyCode m_RunKey = KeyCode.LeftShift;

        [Header("Cameras")]
        [SerializeField] private Camera m_ThirdPersonCamera;
        [SerializeField] private UnityEngine.Camera m_FirstPersonCamera;

        [Header("Mouse Inputs")]
        [SerializeField] private string m_MouseX = "Mouse X";
        [SerializeField] private string m_MouseY = "Mouse Y";
        [SerializeField] private string m_MouseScroll = "Mouse ScrollWheel";

        private CreatureMover m_Mover;
        private Vector2 m_Axis;
        private bool m_IsRun;
        private bool m_IsJump;
        private Vector3 m_Target;
        private Vector2 m_MouseDelta;
        private float m_Scroll;

        private bool isFirstPerson = false;
        public bool canMove = true;
        public bool lockCursor = true; // Can be toggled by UI/Fail screens

        private void Awake()
        {
            m_Mover = GetComponent<CreatureMover>();

            switch (startingCamera)
            {
                case StartingCameraMode.FirstPerson:
                    SetCameraState(true);
                    break;
                case StartingCameraMode.ThirdPerson:
                    SetCameraState(false);
                    break;
                case StartingCameraMode.LeaveForCutscene:
                    // Does not touch camera activation; allows Timelines or custom setups to run unhindered
                    break;
            }

            if (footstepSource != null)
            {
                footstepSource.loop = true;
            }
        }

        private void Update()
        {
            // Only enforce cursor lock while playing gameplay
            if (lockCursor)
            {
                if (Cursor.lockState != CursorLockMode.Locked)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }

            GatherInput();
            SetInput();
            HandleAudio();
        }

        private void HandleAudio()
        {
            if (!canMove)
            {
                if (footstepSource != null && footstepSource.isPlaying) footstepSource.Stop();
                return;
            }

            if (footstepSource != null)
            {
                if (m_Axis.sqrMagnitude > 0.1f && m_IsRun)
                {
                    if (footstepSource.clip != runLoop) footstepSource.clip = runLoop;
                    if (!footstepSource.isPlaying) footstepSource.Play();
                }
                else
                {
                    if (footstepSource.isPlaying) footstepSource.Stop();
                }
            }

            if (TaskManager.Instance != null && TaskManager.Instance.currentPhase == TaskManager.TaskPhase.Herding)
            {
                if (barkSource != null && barkSound != null && Input.GetMouseButtonDown(0))
                {
                    barkSource.PlayOneShot(barkSound);
                }
            }
        }

        public void SetCameraState(bool state)
        {
            isFirstPerson = state;

            if (m_ThirdPersonCamera != null) m_ThirdPersonCamera.gameObject.SetActive(!isFirstPerson);
            if (m_FirstPersonCamera != null) m_FirstPersonCamera.gameObject.SetActive(isFirstPerson);
        }

        public void GatherInput()
        {
            if (canMove)
            {
                m_Axis = new Vector2(Input.GetAxis(m_HorizontalAxis), Input.GetAxis(m_VerticalAxis));
                m_IsRun = Input.GetKey(m_RunKey);
                m_IsJump = Input.GetButton(m_JumpButton);
            }
            else
            {
                m_Axis = Vector2.zero;
                m_IsRun = false;
                m_IsJump = false;
            }

            // If cursor is free to click UI, stop rotating the camera
            if (!lockCursor)
            {
                m_MouseDelta = Vector2.zero;
                return;
            }

            float verticalMouse = m_MouseY == "" ? 0 : Input.GetAxis(m_MouseY);
            m_MouseDelta = new Vector2(Input.GetAxis(m_MouseX), -verticalMouse);
            m_Scroll = Input.GetAxis(m_MouseScroll);

            if (isFirstPerson)
            {
                m_Target = (m_FirstPersonCamera == null) ? Vector3.zero : m_FirstPersonCamera.transform.position + m_FirstPersonCamera.transform.forward * 10f;
            }
            else
            {
                m_Target = (m_ThirdPersonCamera == null) ? Vector3.zero : m_ThirdPersonCamera.Target;
            }
        }

        public void BindMover(CreatureMover mover)
        {
            m_Mover = mover;
        }

        public void SetInput()
        {
            if (m_Mover != null)
            {
                m_Mover.SetInput(in m_Axis, in m_Target, in m_IsRun, m_IsJump);
            }

            if (!isFirstPerson && m_ThirdPersonCamera != null && lockCursor)
            {
                m_ThirdPersonCamera.SetInput(in m_MouseDelta, m_Scroll);
            }
        }
    }
}