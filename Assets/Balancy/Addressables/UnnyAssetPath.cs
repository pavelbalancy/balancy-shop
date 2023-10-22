#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Balancy.Addressables
{
    public class UnnyAssetPath : MonoBehaviour
    {
#if UNITY_EDITOR
        protected string GetAssetPath(Object asset)
        {
            return AssetDatabase.GetAssetPath(asset);
        }
#endif
    }
}