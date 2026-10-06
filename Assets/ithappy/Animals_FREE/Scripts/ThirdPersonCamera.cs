using UnityEngine;

namespace ithappy.Animals_FREE
{
    public class ThirdPersonCamera : Camera
    {
        [SerializeField, Range(0f, 2f)]
        private float m_Offset = 1.5f;
        [SerializeField, Range(0f, 360f)]
        private float m_CameraSpeed = 90f;
        [SerializeField] private LayerMask cameraCollisionLayers;

        private Vector3 m_LookPoint;
        private Vector3 m_TargetPos;

        private void LateUpdate()
        {
            Move(Time.deltaTime);
        }

        public override void SetInput(in Vector2 delta, float scroll)
        {
            base.SetInput(delta, scroll);

            var dir = new Vector3(0, 0, -m_Distance);
            var rot = Quaternion.Euler(m_Angles.x, m_Angles.y, 0f);

            var playerPos = (m_Player == null) ? Vector3.zero : m_Player.position;
            m_LookPoint = playerPos + m_Offset * Vector3.up;
            m_TargetPos = m_LookPoint + rot * dir;
        }

        private void Move(float deltaTime)
        {
            camera();
            target();

            void camera()
            {
                if (Physics.Linecast(m_LookPoint, m_TargetPos, out RaycastHit hit, cameraCollisionLayers, QueryTriggerInteraction.Ignore))
                {
                    m_Transform.position = hit.point + hit.normal * 0.1f;
                }
                else
                {
                    m_Transform.position = m_TargetPos;
                }

                m_Transform.LookAt(m_LookPoint);
            }

            void target()
            {
                if(m_Target == null)
                {
                    return;
                }

                m_Target.position = m_Transform.position + m_Transform.forward * TargetDistance;
            }
        }
    }
}