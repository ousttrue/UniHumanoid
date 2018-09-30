using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace UniHumanoid
{
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
            var index = (int)bone;
            if (index < 0) return -1;
            if (index >= _boneIndices.Length) return -1;
            return _boneIndices[(int)bone];
        }

        public string GetBoneName(HumanBodyBones bone)
        {
            return _boneNames[(int)bone];
        }

        public Dictionary<HumanBodyBones, T> ToDictionary<T>(T[] values)
        {
            var self = this;
            return ((HumanBodyBones[])Enum.GetValues(typeof(HumanBodyBones)))
                .Where(x => self.GetBoneIndex(x) >= 0)
                .ToDictionary(x => x, x => values[self.GetBoneIndex(x)])
                ;
        }
    }
}
