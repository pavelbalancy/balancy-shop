using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Balancy.Addressables
{
    public class AssetsLoader
    {
        private class LoadedObject
        {
            private readonly Object _originalObject;

            public LoadedObject(Object originalObject)
            {
                _originalObject = originalObject;
            }

            public Object GetObject()
            {
                return _originalObject;
            }
        }

        private static readonly Transform m_PoolHolder;

        private static readonly Dictionary<string, LoadedObject> _loadedObjects = new Dictionary<string, LoadedObject>(16);
        private static readonly Dictionary<string, List<Action<Object>>> _loadingQueue = new Dictionary<string, List<Action<Object>>>(16);

        public static void GetSprite(UnnyAsset asset, Action<Sprite> callback)
        {
            GetSprite(asset.Name, callback);
        }

        public static void GetSprite(string name, Action<Sprite> callback)
        {
            CheckAndPrepareObject<Sprite>(name, o => callback(o as Sprite));
        }

        public static void GetObject(string name, Action<Object> callback)
        {
            CheckAndPrepareObject<Object>(name, callback);
        }

        private static void CheckAndPrepareObject<T>(string name, Action<Object> callback) where T : Object
        {
            if (_loadedObjects.TryGetValue(name, out var value))
            {
                callback?.Invoke(value?.GetObject());
            }
            else
            {
                if (_loadingQueue.TryGetValue(name, out var queue))
                {
                    queue?.Add(callback);
                }
                else
                {
                    var newQueue = new List<Action<Object>> {callback};
                    _loadingQueue.Add(name, newQueue);

                    UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<T>(name).Completed += result =>
                    {
                        if (result.Status == AsyncOperationStatus.Succeeded)
                        {
                            var loadedObject = new LoadedObject(result.Result);
                            _loadedObjects.Add(name, loadedObject);
                            var obj = loadedObject.GetObject();
                            foreach (var action in newQueue)
                                action(obj);
                        }
                        else
                        {
                            Debug.LogError("Couldn't load asset by name " + name);
                            foreach (var action in newQueue)
                                action(null);
                        }
                        
                        _loadingQueue.Remove(name);
                    };
                }
            }
        }
    }
}