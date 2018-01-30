using System.IO;
using UnityEditor;
using UnityEngine;


namespace UniHumanoid
{
    public class bvhAssetPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string path in importedAssets)
            {
                var ext = Path.GetExtension(path).ToLower();
                if (ext == ".bvh")
                {
                    ImportBvh(path);
                }
            }
        }

        static void ImportBvh(string srcPath)
        {
            Debug.LogFormat("ImportBvh: {0}", srcPath);

            var src = File.ReadAllText(srcPath, System.Text.Encoding.UTF8);
            var bvh = Bvh.Parse(src);
            Debug.LogFormat("parsed {0}", bvh);

            using (var context = new PrefabContext(srcPath))
            {
                var root = new GameObject(Path.GetFileNameWithoutExtension(srcPath));

                context.SetMainGameObject(root.name, root);

                var toMeter = 0.03f;
                BuildHierarchy(root.transform, bvh.Root, toMeter);

                var minY = 0.0f;
                foreach(var x in root.transform.Traverse())
                {
                    if (x.position.y < minY)
                    {
                        minY = x.position.y;
                    }
                }

                // foot height to 0
                root.transform.GetChild(0).position = new Vector3(0, -minY, 0);

                var clip = CreateAnimationClip(root.transform, bvh);
                clip.name = root.name;
                clip.legacy = true;
                clip.wrapMode = WrapMode.Loop;
                var animation = root.AddComponent<Animation>();
                animation.AddClip(clip, clip.name);
                animation.clip = clip;
                animation.Play();
            }
        }

        static AnimationClip CreateAnimationClip(Transform root, Bvh bvh)
        {
            var clip = new AnimationClip();

            foreach(var channel in bvh.Channels)
            {
                var curve = new AnimationCurve();
                foreach(var key in channel.Keys)
                {

                }
                //clip.SetCurve();
            }

            return clip;
        }

        static void BuildHierarchy(Transform parent, BvhNode node, float toMeter)
        {
            var go = new GameObject(node.Name);
            go.transform.localPosition = node.Offset * toMeter;
            go.transform.SetParent(parent, false);

            var gizmo=go.AddComponent<BoneGizmoDrawer>();

            foreach(var child in node.Children)
            {
                BuildHierarchy(go.transform, child, toMeter);
            }
        }
    }
}
