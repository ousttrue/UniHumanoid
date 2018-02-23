using UnityEngine;


namespace UniHumanoid
{
    public class HumanPoseClip : ScriptableObject
    {
        public Vector3 bodyPosition;

        public Quaternion bodyRotation;

        public float[] muscles;

        public void GetPose(out HumanPose pose)
        {
            pose = new HumanPose
            {
                bodyPosition = bodyPosition,
                bodyRotation = bodyRotation,
                muscles = muscles
            };
        }

        public void SetPose(ref HumanPose pose)
        {
            bodyPosition = pose.bodyPosition;
            bodyRotation = pose.bodyRotation;
            muscles = (float[])pose.muscles.Clone();
        }
    }
}
