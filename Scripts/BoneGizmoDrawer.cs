using UnityEditor;
using UnityEngine;


namespace UniHumanoid
{
    public class BoneGizmoDrawer : MonoBehaviour
    {
        readonly Vector3 SIZE = new Vector3(0.05f, 0.05f, 0.05f);

        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(transform.position, SIZE);

            Handles.Label(transform.position, name);
        }
    }
}
