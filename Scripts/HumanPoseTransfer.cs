using UnityEngine;


namespace UniHumanoid
{
    public class HumanPoseTransfer : MonoBehaviour
    {
        [SerializeField]
        public Avatar Avatar;
        private void Reset()
        {
            var animator = GetComponent<Animator>();
            if (animator != null)
            {
                Avatar = animator.avatar;
            }
        }

        [SerializeField]
        HumanPoseTransfer m_source;

        [SerializeField]
        HumanPoseClip m_poseClip;

        HumanPoseHandler m_handler;
        private void Awake()
        {
            Setup();
        }

        public void Setup()
        {
            if (Avatar == null)
            {
                return;
            }
            m_handler = new HumanPoseHandler(Avatar, transform);
        }

        HumanPose m_pose;

        int m_lastFrameCount = -1;

        public bool GetPose(int frameCount, out HumanPose pose)
        {
            if (m_poseClip != null)
            {
                m_poseClip.GetPose(out pose);
                return true;
            }

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
