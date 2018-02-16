using System;
using UnityEngine;


namespace UniHumanoid
{
    public interface IImporterContext : IDisposable
    {
        String Path { get; }
        GameObject Root { get; }
        void SetMainGameObject(string key, GameObject go);
        void AddObjectToAsset(string key, UnityEngine.Object o);
    }

    public class RuntimeContext : IImporterContext
    {
        public string Path
        {
            get;
            private set;
        }

        public GameObject Root
        {
            get;
            private set;
        }

        public void Dispose()
        {
        }

        public RuntimeContext(String path)
        {
            Path = path;
        }

        public void AddObjectToAsset(string key, UnityEngine.Object o)
        {
        }

        public void SetMainGameObject(string key, GameObject go)
        {
            Root = go;
        }
    }
}
