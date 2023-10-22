using UnityEngine;

namespace Balancy.Addressables
{
    public class UnnyAssetLink : UnnyAssetPath, IUnnyAsset
    {
        [SerializeField]
        private Texture2D _texture = null;

#if UNITY_EDITOR
        public string GetPreviewImagePath()
        {
            return GetAssetPath(_texture);
        }
#endif
    }
}