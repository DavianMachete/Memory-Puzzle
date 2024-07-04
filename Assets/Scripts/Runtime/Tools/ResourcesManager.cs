using System;
using System.Collections.Generic;

using UnityEngine;

using Object = UnityEngine.Object;

namespace MP.Tools
{
    public static class ResourcesManager
    {
        private const string EngineAssetsDir = "EngineAssets";
        private const string AssetsDir = "Assets";

        public static Object Load(string path, LoadMode loadMode = LoadMode.PreferGameAssets)
        {
            return Load<Object>(path, loadMode);
        }

        public static T Load<T>(string path, LoadMode loadMode = LoadMode.PreferGameAssets) where T : Object
        {
            switch(loadMode)
            {
                case LoadMode.EngineAssetsOnly:
                    return Resources.Load<T>(GetPath(path, false));
                case LoadMode.GameAssetsOnly:
                    return Resources.Load<T>(GetPath(path, true));
                case LoadMode.PreferEngineAssets:
                    var engineAsset = Resources.Load<T>(GetPath(path, false));
                    // ReSharper disable once Unity.NoNullCoalescing
                    return engineAsset ?? Resources.Load<T>(GetPath(path, true));
                case LoadMode.PreferGameAssets:
                default:
                    var gameAsset = Resources.Load<T>(GetPath(path, true));
                    // ReSharper disable once Unity.NoNullCoalescing
                    return gameAsset ?? Resources.Load<T>(GetPath(path, false));
            }
        }

        public static Object Load(string path, Type type, LoadMode loadMode = LoadMode.PreferGameAssets)
        {
            switch(loadMode)
            {
                case LoadMode.EngineAssetsOnly:
                    return Resources.Load(GetPath(path, false), type);
                case LoadMode.GameAssetsOnly:
                    return Resources.Load(GetPath(path, true), type);
                case LoadMode.PreferEngineAssets:
                    var engineAsset = Resources.Load(GetPath(path, false), type);
                    // ReSharper disable once Unity.NoNullCoalescing
                    return engineAsset ?? Resources.Load(GetPath(path, true), type);
                case LoadMode.PreferGameAssets:
                default:
                    var gameAsset = Resources.Load(GetPath(path, true), type);
                    // ReSharper disable once Unity.NoNullCoalescing
                    return gameAsset ?? Resources.Load(GetPath(path, false), type);
            }
        }

        public static T[] LoadAll<T>(string path, LoadMode loadMode = LoadMode.PreferGameAssets) where T:Object
        {
            switch(loadMode)
            {
                case LoadMode.EngineAssetsOnly:
                    return Resources.LoadAll<T>(GetPath(path, false));
                case LoadMode.GameAssetsOnly:
                    return Resources.LoadAll<T>(GetPath(path, true));
            }

            var engineAssets = Resources.LoadAll<T>(GetPath(path, false));
            var gameAssets = Resources.LoadAll<T>(GetPath(path, true));

            var assets = loadMode == LoadMode.PreferGameAssets
                ? MergeAssetsByPriority(gameAssets, engineAssets)
                : MergeAssetsByPriority(engineAssets, gameAssets);

            return assets.ToArray();
        }

        public static Object[] LoadAll(string path, Type type, LoadMode loadMode = LoadMode.PreferGameAssets)
        {
            switch(loadMode)
            {
                case LoadMode.EngineAssetsOnly:
                    return Resources.LoadAll(GetPath(path, false), type);
                case LoadMode.GameAssetsOnly:
                    return Resources.LoadAll(GetPath(path, true), type);
            }

            var engineAssets = Resources.LoadAll(GetPath(path, false), type);
            var gameAssets = Resources.LoadAll(GetPath(path, true), type);

            var assets = loadMode == LoadMode.PreferGameAssets
                ? MergeAssetsByPriority(gameAssets, engineAssets)
                : MergeAssetsByPriority(engineAssets, gameAssets);

            return assets.ToArray();
        }

        private static List<T> MergeAssetsByPriority<T>(T[] highPriority, T[] lowPriority)
            where T:Object
        {
            // Cache all low priority assets by their name
            var lowPriorityHashed = new Dictionary<string, T>(lowPriority.Length);
            foreach (var asset in lowPriority)
            {
                lowPriorityHashed[asset.name] = asset;
            }

            // Remove any low priority assets that are in the high priority assets collection
            foreach (var asset in highPriority)
            {
                lowPriorityHashed.Remove(asset.name);
            }

            // Combine the high priority assets and the remaining low priority assets
            var combined = new List<T>(highPriority);
            combined.AddRange(lowPriorityHashed.Values);

            return combined;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static string GetPath(string path, bool isGamePath)
        {
            return string.Format((isGamePath ? AssetsDir : EngineAssetsDir) + "/" + path);
        }

        public enum LoadMode
        {
            PreferGameAssets,
            PreferEngineAssets,
            GameAssetsOnly,
            EngineAssetsOnly
        }
    }
}