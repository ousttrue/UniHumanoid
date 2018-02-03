using UnityEngine;


namespace UniHumanoid
{
    public class HumanPoseTransfer : MonoBehaviour
    {
        [SerializeField]
        HumanPoseTransfer m_source;

        HumanPoseHandler m_handler;
        private void Awake()
        {
            var animator = GetComponent<Animator>();
            if (animator != null)
            {
                if (animator.avatar != null)
                {
                    m_handler = new HumanPoseHandler(animator.avatar, transform);
                }
            }
        }

        HumanPose m_pose;

        int m_lastFrameCount = -1;

        public bool GetPose(int frameCount, out HumanPose pose)
        {
            if (m_handler == null)
            {
                pose = m_pose;
                return false;
            }

            if (frameCount != m_lastFrameCount)
            {
                m_handler.GetHumanPose(ref m_pose);
                m_lastFrameCount = frameCount;
            }
            pose = m_pose;
            return true;
        }

        private void Update()
        {
            if (m_source == null)
            {
                return;
            }
            if (m_handler == null)
            {
                return;
            }

            if(m_source.GetPose(Time.frameCount, out m_pose))
            {
                m_handler.SetHumanPose(ref m_pose);
            }
        }
    }
}
