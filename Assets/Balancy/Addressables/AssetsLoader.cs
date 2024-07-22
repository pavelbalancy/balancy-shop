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

        public static AsyncLoadHandler GetSprite(UnnyAsset asset, Action<Sprite> callback)
        {
            return GetSprite(asset.Name, callback);
        }

        public static AsyncLoadHandler GetSprite(string name, Action<Sprite> callback)
        {
            var handler = AsyncLoadHandler.CreateHandler();
            CheckAndPrepareObject<Sprite>(name, o =>
            {
                if (handler.GetStatus() == AsyncLoadHandler.Status.Loading)
                {
                    handler.Finish();
                    callback(o as Sprite);
                }
            });
            return handler;
        }

        public static AsyncLoadHandler GetObject(string name, Action<Object> callback)
        {
            var handler = AsyncLoadHandler.CreateHandler();
            CheckAndPrepareObject<Object>(name, o =>
            {
                if (handler.GetStatus() == AsyncLoadHandler.Status.Loading)
                {
                    handler.Finish();
                    callback(o);
                }
            });
            return handler;
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
                        Object obj = null;
                        if (result.Status == AsyncOperationStatus.Succeeded)
                        {
                            var loadedObject = new LoadedObject(result.Result);
                            _loadedObjects.Add(name, loadedObject);
                            obj = loadedObject.GetObject();
                        }
                        else
                        {
                            Debug.LogError("Couldn't load asset by name " + name);
                        }

                        foreach (var action in newQueue)
                            action(obj);

                        _loadingQueue.Remove(name);
                    };
                }
            }
        }
    }
}