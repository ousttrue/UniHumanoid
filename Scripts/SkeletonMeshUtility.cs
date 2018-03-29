using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniHumanoid
{
    public static class SkeletonMeshUtility
    {
        class MeshBuilder
        {
            List<Vector3> m_positioins = new List<Vector3>();
            List<int> m_indices = new List<int>();
            List<BoneWeight> m_boneWeights = new List<BoneWeight>();

            public void AddBone(Vector3 head, Vector3 tail, int boneIndex)
            {
                // ToDo
            }

            public Mesh CreateMesh()
            {
                var mesh = new Mesh();
                mesh.SetVertices(m_positioins);
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();
                mesh.boneWeights = m_boneWeights.ToArray();
                mesh.triangles = m_indices.ToArray();
                return mesh;
            }
        }

        struct BoneHeadTail
        {
            public HumanBodyBones Head;
            public HumanBodyBones Tail;

            public BoneHeadTail(HumanBodyBones head, HumanBodyBones tail)
            {
                Head = head;
                Tail = tail;
            }
        }

        static BoneHeadTail[] Bones = new BoneHeadTail[]
        {
            // ToDo
            new BoneHeadTail(HumanBodyBones.Hips, HumanBodyBones.Spine),
        };

        public static SkinnedMeshRenderer CreateRenderer(Animator animator)
        {
            var bodyBones = (HumanBodyBones[])Enum.GetValues(typeof(HumanBodyBones));
            var bones = animator.transform.Traverse().ToList();

            var builder = new MeshBuilder();
            foreach(var headTail in Bones)
            {
                var head = animator.GetBoneTransform(headTail.Head);
                var tail = animator.GetBoneTransform(headTail.Tail);
                if (head!=null && tail!=null)
                {
                    builder.AddBone(head.position,  tail.position, bones.IndexOf(head));
                }
            }

            var mesh = builder.CreateMesh();
            mesh.bindposes = bones.Select(x =>
                            x.worldToLocalMatrix * animator.transform.localToWorldMatrix).ToArray();
            var renderer = animator.gameObject.AddComponent<SkinnedMeshRenderer>();
            renderer.sharedMesh = mesh;
            renderer.bones = bones.ToArray();
            return renderer;
        }
    }
}
