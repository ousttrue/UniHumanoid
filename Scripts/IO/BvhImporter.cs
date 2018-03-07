using System.IO;
using System.Text;
using UnityEngine;


namespace UniHumanoid
{
    public static class BvhImporter
    {
        public static void Import(IImporterContext context)
        {
            //
            // parse
            //
            var src = File.ReadAllText(context.Path, Encoding.UTF8);
            var bvh = Bvh.Parse(src);
            Debug.LogFormat("parsed {0}", bvh);

            //
            // build hierarchy
            //
            var root = new GameObject(Path.GetFileNameWithoutExtension(context.Path));
            root.AddComponent<Animator>();

            context.SetMainGameObject(root.name, root);

            BuildHierarchy(root.transform, bvh.Root, 1.0f);

            var minY = 0.0f;
            foreach (var x in root.transform.Traverse())
            {
                if (x.position.y < minY)
                {
                    minY = x.position.y;
                }
            }

            var toMeter = 1.0f / (-minY);
            Debug.LogFormat("minY: {0} {1}", minY, toMeter);
            foreach (var x in root.transform.Traverse())
            {
                x.localPosition *= toMeter;
            }

            // foot height to 0
            var hips = root.transform.GetChild(0);
            hips.position = new Vector3(0, -minY * toMeter, 0);

            //
            // create AnimationClip
            //
            var clip = BvhAnimation.CreateAnimationClip(bvh, toMeter);
            clip.name = root.name;
            clip.legacy = true;
            clip.wrapMode = WrapMode.Loop;
            context.AddObjectToAsset(clip.name, clip);

            var animation = root.AddComponent<Animation>();
            animation.AddClip(clip, clip.name);
            animation.clip = clip;
            animation.Play();

            var boneMapping = root.AddComponent<BoneMapping>();
            boneMapping.Bones[(int)HumanBodyBones.Hips] = hips.gameObject;
            boneMapping.GuessBoneMapping();
            var avatarWithDescription = boneMapping.CreateAvatar();
            context.AddObjectToAsset(avatarWithDescription.Avatar.name, avatarWithDescription.Avatar);
            context.AddObjectToAsset(avatarWithDescription.Description.name, avatarWithDescription.Description);

            var humanPoseTransfer = root.AddComponent<HumanPoseTransfer>();
            humanPoseTransfer.Avatar = avatarWithDescription.Avatar;
        }

        static void BuildHierarchy(Transform parent, BvhNode node, float toMeter)
        {
            var go = new GameObject(node.Name);
            go.transform.localPosition = node.Offset.ToXReversedVector3() * toMeter;
            go.transform.SetParent(parent, false);

            var gizmo = go.AddComponent<BoneGizmoDrawer>();
            gizmo.Draw = true;

            foreach (var child in node.Children)
            {
                BuildHierarchy(go.transform, child, toMeter);
            }
        }
    }
}
