using UnityEngine;


namespace UniHumanoid
{
    public class HumanPoseTransfer : MonoBehaviour
    {
        public enum HumanPoseTransferSourceType
        {
            HumanPoseHandler,
            HumanPoseClip,
        }

        [SerializeField]
        public HumanPoseTransferSourceType SourceType;

        [SerializeField]
        public Avatar Avatar;

        #region Standalone
        public HumanPose CreatePose()
        {
            var handler = new HumanPoseHandler(Avatar, transform);
            var pose = default(HumanPose);
            handler.GetHumanPose(ref pose);
            return pose;
        }
        public void SetPose(HumanPose pose)
        {
            var handler = new HumanPoseHandler(Avatar, transform);
            handler.SetHumanPose(ref pose);
        }
        #endregion

        private void Reset()
        {
            var animator = GetComponent<Animator>();
            if (animator != null)
            {
                Avatar = animator.avatar;
            }
        }

        [SerializeField]
        public HumanPoseTransfer Source;

        [SerializeField]
        public HumanPoseClip PoseClip;

        HumanPoseHandler m_handler;
        private void Awake()
        {
            var animator = GetComponent<Animator>();
            if (animator != null)
            {
                Avatar = animator.avatar;
            }

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
            if (PoseClip != null)
            {
                PoseClip.GetPose(out pose);
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
            if (Source == null)
            {
                return;
            }
            if (m_handler == null)
            {
                return;
            }

            if(Source.GetPose(Time.frameCount, out m_pose))
            {
                m_handler.SetHumanPose(ref m_pose);
            }
        }
    }
}
