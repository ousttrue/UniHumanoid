using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace UniHumanoid
{
    public interface IBone
    {
        string Name { get; }
        Vector3 SkeletonLoacalPosition { get; }
        IBone Parent { get; }
        IList<IBone> Children { get; }
    }

    public static class IBoneExtensions
    {
        public static IEnumerable<IBone> Traverse(this IBone self)
        {
            yield return self;
            foreach (var child in self.Children)
            {
                foreach (var x in child.Traverse())
                {
                    yield return x;
                }
            }
        }

        public static Vector3 CenterOfDescendant(this IBone self)
        {
            var sum = Vector3.zero;
            int i = 0;
            foreach (var x in self.Traverse())
            {
                sum += x.SkeletonLoacalPosition;
                ++i;
            }
            return sum / i;
        }
    }

    public class BvhBone : IBone
    {
        public string Name
        {
            private set;
            get;
        }

        public Vector3 SkeletonLoacalPosition
        {
            private set;
            get;
        }

        public BvhBone(string name, Vector3 position)
        {
            Name = name;
            SkeletonLoacalPosition = position;
        }

        public override string ToString()
        {
            return string.Format("<BvhBone: {0}>", Name);
        }

        public IBone Parent
        {
            private set;
            get;
        }

        List<IBone> _children = new List<IBone>();
        public IList<IBone> Children
        {
            get { return _children; }
        }

        public void Build(BvhNode node)
        {
            foreach (var child in node.Children)
            {
                var childBone = new BvhBone(child.Name, SkeletonLoacalPosition + child.Offset.ToXReversedVector3());
                childBone.Parent = this;
                _children.Add(childBone);

                childBone.Build(child);
            }
        }

        public IEnumerable<BvhBone> Traverse()
        {
            yield return this;
            foreach (var child in Children)
            {
                foreach (var x in child.Traverse())
                {
                    yield return (BvhBone)x;
                }
            }
        }
    }


    public struct Skeleton
    {
        int[] _boneIndices;
        string[] _boneNames;

        public void Set(HumanBodyBones bone, IList<IBone> bones, IBone b)
        {
            if (b != null)
            {
                Set(bone, bones.IndexOf(b), b.Name);
            }
        }

        public void Set(HumanBodyBones bone, int index, string name)
        {
            if (_boneIndices == null)
            {
                _boneIndices = new int[(int)HumanBodyBones.LastBone];
            }
            _boneIndices[(int)bone] = index;
            if (_boneNames == null)
            {
                _boneNames = new string[(int)HumanBodyBones.LastBone];
            }
            _boneNames[(int)bone] = name;
        }

        public int GetBoneIndex(HumanBodyBones bone)
        {
            return _boneIndices[(int)bone];
        }

        public string GetBoneName(HumanBodyBones bone)
        {
            return _boneNames[(int)bone];
        }
    }


    public interface ISkeletonDetector
    {
        Skeleton Detect(IList<IBone> bones);
    }


    public class BvhSkeletonEstimator : ISkeletonDetector
    {
        static IBone GetRoot(IList<IBone> bones)
        {
            var hips = bones.Where(x => x.Parent == null).ToArray();
            if (hips.Length != 1)
            {
                throw new System.Exception("Require unique root");
            }
            return hips[0];
        }

        static IBone SelectBone(Func<IBone, IBone, IBone> selector, IList<IBone> bones)
        {
            if (bones == null || bones.Count == 0) throw new Exception("no bones");
            var current = bones[0];
            for (var i = 1; i < bones.Count; ++i)
            {
                current = selector(current, bones[i]);
            }
            return current;
        }

        static void GetSpineAndHips(IBone hips, out IBone spine, out IBone leg_L, out IBone leg_R)
        {
            if (hips.Children.Count != 3) throw new System.Exception("Hips require 3 children");
            spine = SelectBone((l, r) => l.SkeletonLoacalPosition.y > r.SkeletonLoacalPosition.y ? l : r, hips.Children);
            leg_L = SelectBone((l, r) => l.SkeletonLoacalPosition.x < r.SkeletonLoacalPosition.x ? l : r, hips.Children);
            leg_R = SelectBone((l, r) => l.SkeletonLoacalPosition.x > r.SkeletonLoacalPosition.x ? l : r, hips.Children);
        }

        static void GetNeckAndArms(IBone chest, out IBone neck, out IBone arm_L, out IBone arm_R)
        {
            if (chest.Children.Count != 3) throw new System.Exception("Chest require 3 children");
            neck = SelectBone((l, r) => l.CenterOfDescendant().y > r.CenterOfDescendant().y ? l : r, chest.Children);
            arm_L = SelectBone((l, r) => l.CenterOfDescendant().x < r.CenterOfDescendant().x ? l : r, chest.Children);
            arm_R = SelectBone((l, r) => l.CenterOfDescendant().x > r.CenterOfDescendant().x ? l : r, chest.Children);
        }

        struct Arm
        {
            public IBone Shoulder;
            public IBone UpperArm;
            public IBone LowerArm;
            public IBone Hand;
        }

        Arm GetArm(IBone shoulder)
        {
            var bones = shoulder.Traverse().ToArray();
            switch (bones.Length)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    throw new NotImplementedException();

                default:
                    return new Arm
                    {
                        Shoulder = bones[0],
                        UpperArm = bones[1],
                        LowerArm = bones[2],
                        Hand = bones[3],
                    };
            }
        }

        struct Leg
        {
            public IBone UpperLeg;
            public IBone LowerLeg;
            public IBone Foot;
            public IBone Toes;
        }

        Leg GetLeg(IBone leg)
        {
            var bones = leg.Traverse().ToArray();
            switch (bones.Length)
            {
                case 0:
                case 1:
                case 2:
                    throw new NotImplementedException();

                case 3:
                    return new Leg
                    {
                        UpperLeg = bones[0],
                        LowerLeg = bones[1],
                        Foot = bones[2],
                    };

                default:
                    return new Leg
                    {
                        UpperLeg = bones[0],
                        LowerLeg = bones[1],
                        Foot = bones[2],
                        Toes = bones[3],
                    };
            }
        }

        public Skeleton Detect(IList<IBone> bones)
        {
            //
            // search bones
            //
            var hips = GetRoot(bones);

            IBone spine, hip_L, hip_R;
            GetSpineAndHips(hips, out spine, out hip_L, out hip_R);
            var legLeft = GetLeg(hip_L);
            var legRight = GetLeg(hip_R);

            IBone chest = spine.Traverse().First(x => x.Children.Count == 3);
            if (spine == chest)
            {
                spine = null;
            }

            IBone neck, shoulder_L, shoulder_R;
            GetNeckAndArms(chest, out neck, out shoulder_L, out shoulder_R);
            var armLeft = GetArm(shoulder_L);
            var armRight = GetArm(shoulder_R);

            var head = neck.Children.First();

            //
            //  set result
            //
            var skeleton = new Skeleton();
            skeleton.Set(HumanBodyBones.Hips, bones, hips);
            skeleton.Set(HumanBodyBones.Spine, bones, spine);
            skeleton.Set(HumanBodyBones.Chest, bones.IndexOf(chest), chest.Name);
            skeleton.Set(HumanBodyBones.Neck, bones, neck);
            skeleton.Set(HumanBodyBones.Head, bones, head);

            skeleton.Set(HumanBodyBones.LeftUpperLeg, bones, legLeft.UpperLeg);
            skeleton.Set(HumanBodyBones.LeftLowerLeg, bones, legLeft.LowerLeg);
            skeleton.Set(HumanBodyBones.LeftFoot, bones, legLeft.Foot);
            skeleton.Set(HumanBodyBones.LeftToes, bones, legLeft.Toes);

            skeleton.Set(HumanBodyBones.RightUpperLeg, bones, legRight.UpperLeg);
            skeleton.Set(HumanBodyBones.RightLowerLeg, bones, legRight.LowerLeg);
            skeleton.Set(HumanBodyBones.RightFoot, bones, legRight.Foot);
            skeleton.Set(HumanBodyBones.RightToes, bones, legRight.Toes);

            skeleton.Set(HumanBodyBones.LeftShoulder, bones, armLeft.Shoulder);
            skeleton.Set(HumanBodyBones.LeftUpperArm, bones, armLeft.UpperArm);
            skeleton.Set(HumanBodyBones.LeftLowerArm, bones, armLeft.LowerArm);
            skeleton.Set(HumanBodyBones.LeftHand, bones, armLeft.Hand);

            skeleton.Set(HumanBodyBones.RightShoulder, bones, armRight.Shoulder);
            skeleton.Set(HumanBodyBones.RightUpperArm, bones, armRight.UpperArm);
            skeleton.Set(HumanBodyBones.RightLowerArm, bones, armRight.LowerArm);
            skeleton.Set(HumanBodyBones.RightHand, bones, armRight.Hand);

            return skeleton;
        }

        public Skeleton Detect(Bvh bvh)
        {
            var root = new BvhBone(bvh.Root.Name, Vector3.zero);
            root.Build(bvh.Root);
            return Detect(root.Traverse().Select(x => (IBone)x).ToList());
        }
    }
}
