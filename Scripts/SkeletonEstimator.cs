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
            foreach(var child in self.Children)
            {
                foreach(var x in child.Traverse())
                {
                    yield return x;
                }
            }
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
                var childBone = new BvhBone(child.Name, SkeletonLoacalPosition + child.Offset.ToVector3());
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
            for(var i=1; i<bones.Count; ++i)
            {
                current = selector(current, bones[i]);
            }
            return current;
        }

        static void GetSpineAndLegs(IBone hips, out IBone spine, out IBone leg_L, out IBone leg_R)
        {
            if (hips.Children.Count != 3) throw new System.Exception("Hips require 3 children");
            spine = SelectBone((l, r) => l.SkeletonLoacalPosition.y > r.SkeletonLoacalPosition.y ? l : r, hips.Children);
            leg_L = SelectBone((l, r) => l.SkeletonLoacalPosition.x < r.SkeletonLoacalPosition.x ? l : r, hips.Children);
            leg_R = SelectBone((l, r) => l.SkeletonLoacalPosition.x > r.SkeletonLoacalPosition.x ? l : r, hips.Children);
        }

        public Skeleton Detect(IList<IBone> bones)
        {
            var hips = GetRoot(bones);

            IBone spine, leg_L, leg_R;
            GetSpineAndLegs(hips, out spine, out leg_L, out leg_R);

            IBone chest = spine.Traverse().First(x => x.Children.Count == 3);
            if (spine == chest)
            {
                spine = null;
            }

            var boneIndices = new int[(int)HumanBodyBones.LastBone];
            var boneNames = new string[(int)HumanBodyBones.LastBone];

            var skeleton = new Skeleton();

            skeleton.Set(HumanBodyBones.Hips, bones.IndexOf(hips), hips.Name);

            skeleton.Set(HumanBodyBones.LeftUpperLeg, bones.IndexOf(leg_L), leg_L.Name);
            skeleton.Set(HumanBodyBones.RightUpperLeg, bones.IndexOf(leg_R), leg_R.Name);

            if (spine != null)
            {
                skeleton.Set(HumanBodyBones.Spine, bones.IndexOf(spine), spine.Name);
            }
            skeleton.Set(HumanBodyBones.Chest, bones.IndexOf(chest), chest.Name);


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
