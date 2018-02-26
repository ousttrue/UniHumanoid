using System;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace UniHumanoid
{
    [CustomEditor(typeof(HumanPoseTransfer))]
    public class HumanPoseTransferEditor : Editor
    {
        //HumanPoseTransfer m_target;
        SerializedProperty m_avatarProp;
        SerializedProperty m_typeProp;
        SerializedProperty m_clipProp;
        SerializedProperty m_transferProp;

        static string[] SOURCE_TYPES = ((HumanPoseTransfer.HumanPoseTransferSourceType[])
            Enum.GetValues(typeof(HumanPoseTransfer.HumanPoseTransferSourceType)))
            .Select(x => x.ToString())
            .ToArray();

        private void OnEnable()
        {
            //m_target = (HumanPoseTransfer)target;
            m_typeProp = serializedObject.FindProperty("SourceType");
            m_clipProp = serializedObject.FindProperty("PoseClip");
            m_avatarProp = serializedObject.FindProperty("Avatar");
            m_transferProp = serializedObject.FindProperty("Source");
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_avatarProp);

            EditorGUILayout.LabelField("Source");

            m_typeProp.intValue =
                GUILayout.Toolbar(m_typeProp.intValue, SOURCE_TYPES);

            switch ((HumanPoseTransfer.HumanPoseTransferSourceType)m_typeProp.intValue)
            {
                case HumanPoseTransfer.HumanPoseTransferSourceType.HumanPoseClip:
                    PoseClipInspector();
                    break;

                case HumanPoseTransfer.HumanPoseTransferSourceType.HumanPoseHandler:
                    PoseHandler();
                    break;
            }

            // CreatePose
            GUILayout.Space(20);
            if (GUILayout.Button("Create PoseClip"))
            {
                var pose = ((HumanPoseTransfer)serializedObject.targetObject).CreatePose();

                var clip = ScriptableObject.CreateInstance<HumanPoseClip>();
                clip.ApplyPose(ref pose);

                var assetPath = string.Format("Assets/{0}.pose.asset", serializedObject.targetObject.name);
                AssetDatabase.CreateAsset(clip, assetPath);
                Selection.activeObject = clip;
            }
        }

        void PoseClipInspector()
        {
            var old = (HumanPoseClip)m_clipProp.objectReferenceValue;
            EditorGUILayout.PropertyField(m_clipProp);
            serializedObject.ApplyModifiedProperties();

            var _target = (HumanPoseTransfer)target;
            if (_target.PoseClip != old)
            {
                //Debug.Log("clip != old");
                if (_target.PoseClip != null)
                {
                    var pose = _target.PoseClip.GetPose();
                    _target.SetPose(pose);
                }
            }

#if false
            if (_target.PoseClip != null)
            {
                if (GUILayout.Button("Apply PoseClip"))
                {
                    Debug.Log("apply");
                    var pose = default(HumanPose);
                    _target.PoseClip.GetPose(out pose);
                    _target.SetPose(pose);
                }
            }
#endif
        }

        void PoseHandler()
        {
            EditorGUILayout.PropertyField(m_transferProp);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
