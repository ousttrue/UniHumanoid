using System;
using System.Collections.Generic;
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
                
            }
        }
    }
}
