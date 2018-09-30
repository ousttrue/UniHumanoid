using System.Collections.Generic;
using UnityEngine;


namespace UniHumanoid
{
    public interface IBone
    {
        string Name { get; }
        Vector3 SkeletonLocalPosition { get; }
        IBone Parent { get; }
        IList<IBone> Children { get; }
    }
}
