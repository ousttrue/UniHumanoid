using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace UniHumanoid
{
    /*
public static class HumanoidUtility
{
    static Transform GetLeftLeg(Transform[] joints)
    {
        Transform t = null;
        var value = 0.0f;
        foreach (var joint in joints)
        {
            if (t == null 
                || joint.transform.position.x < value
                || joint.GetChild(0).position.x < value
                )
            {
                t = joint;
                value = joint.GetChild(0).position.x;
            }
        }
        return t;
    }

    static Transform GetRightLeg(Transform[] joints)
    {
        Transform t = null;
        var value = 0.0f;
        foreach (var joint in joints)
        {
            if (t == null 
                || joint.transform.position.x > value
                || joint.GetChild(0).position.x > value
                )
            {
                t = joint;
                value = joint.GetChild(0).position.x;
            }
        }
        return t;
    }

    static Transform GetSpine(Transform[] joints)
    {
        Transform t = null;
        foreach (var joint in joints)
        {
            if ((t==null || joint.transform.position.y > t.position.y)
                && !joint.name.ToLower().Contains("hip")
                )
            {
                t = joint;
            }
        }
        return t;
    }

    static Transform GetChest(Transform spine)
    {
        var current = spine;
        while (current != null)
        {
            if (current.childCount >= 3)
            {
                return current;
            }

            if (current.childCount == 0)
            {
                Debug.LogWarningFormat("chest not found: {0}", spine);
                return null;
            }

            current = current.GetChild(0);
        }
        return null;
    }

    static Transform GetLeftArm(Transform chest, Transform[] joints, Vector3 leftDir)
    {
        var values = joints.Select(x => Vector3.Dot((x.position - chest.position).normalized, leftDir)).ToArray();

        var current = joints[0];
        var value = values[0];
        for (int i = 1; i < joints.Length; ++i)
        {
            if (values[i] > value
                || joints[i].name.ToLower().Contains("left")
                )
            {
                value = values[i];
                current = joints[i];
            }
        }
        return current;
    }

    static Transform GetRightArm(Transform chest, Transform[] joints, Vector3 rightDir)
    {
        var values = joints.Select(x => Vector3.Dot((x.position - chest.position).normalized, rightDir)).ToArray();

        var current = joints[0];
        var value = values[0];
        for (int i = 1; i < joints.Length; ++i)
        {
            if (values[i] > value
                || joints[i].name.ToLower().Contains("right"))
            {
                value = values[i];
                current = joints[i];
            }
        }
        return current;
    }

    static Transform GetNeck(Transform[] joints)
    {
        Transform t = joints[0];
        for (int i = 1; i < joints.Length; ++i)
        {
            if (joints[i].transform.position.y > t.position.y)
            {
                t = joints[i];
            }
        }
        return t;
    }

    public static IEnumerable<KeyValuePair<HumanBodyBones, Transform>> TraverseSkeleton(Transform root, Transform[] joints)
    {
        var rootJoints = joints.Where(x => !joints.Contains(x.parent)).ToArray();

        if (rootJoints.Length != 1)
        {
            yield break;
        }

        var hips = rootJoints[0];
        if (hips.childCount < 3)
        {
            yield break;
        }

        var hipsChildren = hips.GetChildren().ToArray();

        var spine = GetSpine(hipsChildren);

        var chest = GetChest(spine);
        var chestChildren = chest.GetChildren().ToArray();

        var neck = GetNeck(chestChildren);
        Transform head = null;
        if (neck.childCount == 0)
        {
            head = neck;
            neck = null;
        }
        else
        {
            head = neck.GetChild(0);
        }

        yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.Hips, hips);

        Func<Transform, bool> SkipBetweenHipsAndUpperLeg = x =>
        {
            // skip bone between hips and upperLegs
            var lowerName = x.name.ToLower();
            return !lowerName.Contains("buttock")
            && !lowerName.Contains("hip")
            ;
        };

        //
        // left leg
        //
        var leftLeg = GetLeftLeg(hipsChildren).Traverse()
            .Where(SkipBetweenHipsAndUpperLeg)
            .ToArray();
        {
            if (leftLeg.Length == 3)
            {
                yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.LeftUpperLeg, leftLeg[0]);
                yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.LeftLowerLeg, leftLeg[1]);
                yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.LeftFoot, leftLeg[2]);
            }
            else if (leftLeg.Length == 4)
            {
                yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.LeftUpperLeg, leftLeg[0]);
                yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.LeftLowerLeg, leftLeg[1]);
                yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.LeftFoot, leftLeg[2]);
                yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.LeftToes, leftLeg[3]);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        //
        // right leg
        //
        var rightLeg = GetRightLeg(hipsChildren).Traverse()
            .Where(SkipBetweenHipsAndUpperLeg)
            .ToArray();
        {
            if (rightLeg.Length == 3)
            {
                yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.RightUpperLeg, rightLeg[0]);
                yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.RightLowerLeg, rightLeg[1]);
                yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.RightFoot, rightLeg[2]);
            }
            else if (rightLeg.Length == 4)
            {
                yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.RightUpperLeg, rightLeg[0]);
                yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.RightLowerLeg, rightLeg[1]);
                yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.RightFoot, rightLeg[2]);
                yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.RightToes, rightLeg[3]);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.Spine, spine);
        if (chest != spine)
        {
            yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.Chest, chest);
        }
        if (neck != null)
        {
            yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.Neck, neck);
        }
        yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.Head, head);

        var rightDir = (rightLeg[0].position - leftLeg[0].position).normalized;

        //
        // left Arm
        //
        {
            var leftArm = GetLeftArm(chest, chestChildren, -rightDir).Traverse().ToArray();

            if (leftArm.Length == 2)
            {
                yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.LeftUpperArm, leftArm[0]);
                yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.LeftLowerArm, leftArm[1]);
            }
            else if (leftArm.Length == 3)
            {
                yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.LeftUpperArm, leftArm[0]);
                yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.LeftLowerArm, leftArm[1]);
                yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.LeftHand, leftArm[2]);
            }
            else if (leftArm.Length >= 4)
            {
                yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.LeftShoulder, leftArm[0]);
                yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.LeftUpperArm, leftArm[1]);
                yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.LeftLowerArm, leftArm[2]);
                yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.LeftHand, leftArm[3]);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        //
        // right Arm
        //
        {
            var rightArm = GetRightArm(chest, chestChildren, rightDir).Traverse().ToArray();

            if (rightArm.Length == 2)
            {
                yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.RightUpperArm, rightArm[0]);
                yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.RightLowerArm, rightArm[1]);
            }
            else if (rightArm.Length == 3)
            {
                yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.RightUpperArm, rightArm[0]);
                yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.RightLowerArm, rightArm[1]);
                yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.RightHand, rightArm[2]);
            }
            else if (rightArm.Length >= 4)
            {
                yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.RightShoulder, rightArm[0]);
                yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.RightUpperArm, rightArm[1]);
                yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.RightLowerArm, rightArm[2]);
                yield return new KeyValuePair<HumanBodyBones, Transform>(HumanBodyBones.RightHand, rightArm[3]);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
    */

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

        public void Build(Transform t)
        {
            foreach(Transform child in t)
            {
                var childBone = new BvhBone(child.name, SkeletonLoacalPosition + child.localPosition);
                childBone.Parent = this;
                _children.Add(childBone);

                childBone.Build(child);
            }
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
                _boneIndices = Enumerable.Repeat(-1, (int)HumanBodyBones.LastBone).ToArray();
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

            var spineToChest = new List<IBone>();
            foreach(var x in spine.Traverse())
            {
                spineToChest.Add(x);
                if (x.Children.Count == 3) break;
            }

            IBone neck, shoulder_L, shoulder_R;
            GetNeckAndArms(spineToChest.Last(), out neck, out shoulder_L, out shoulder_R);
            var armLeft = GetArm(shoulder_L);
            var armRight = GetArm(shoulder_R);

            var head = neck.Children.First();

            //
            //  set result
            //
            var skeleton = new Skeleton();
            skeleton.Set(HumanBodyBones.Hips, bones, hips);

            switch (spineToChest.Count)
            {
                case 0:
                    throw new Exception();

                case 1:
                    skeleton.Set(HumanBodyBones.Spine, bones, spineToChest[0]);
                    break;

                case 2:
                    skeleton.Set(HumanBodyBones.Spine, bones, spineToChest[0]);
                    skeleton.Set(HumanBodyBones.Chest, bones, spineToChest[1]);
                    break;

                case 3:
                    skeleton.Set(HumanBodyBones.Spine, bones, spineToChest[0]);
                    skeleton.Set(HumanBodyBones.Chest, bones, spineToChest[1]);
                    skeleton.Set(HumanBodyBones.UpperChest, bones, spineToChest[2]);
                    break;

                default:
                    skeleton.Set(HumanBodyBones.Spine, bones, spineToChest[0]);
                    skeleton.Set(HumanBodyBones.Chest, bones, spineToChest[1]);
                    skeleton.Set(HumanBodyBones.UpperChest, bones, spineToChest.Last());
                    break;
            }

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

        public Skeleton Detect(Transform t)
        {
            var root = new BvhBone(t.name, Vector3.zero);
            root.Build(t);
            return Detect(root.Traverse().Select(x => (IBone)x).ToList());
        }
    }
}
