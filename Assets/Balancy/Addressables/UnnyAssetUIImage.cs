using UnityEngine;
using UnityEngine.UI;

namespace Balancy.Addressables
{
    public class UnnyAssetUIImage : UnnyAssetPath, IUnnyAsset
    {
#if UNITY_EDITOR
        public string GetPreviewImagePath()
        {
            var image = GetComponentInChildren<Image>();
            if (image == null)
            {
                Debug.LogError("UI.Image wasn't found on " + gameObject.name);
                return null;
            }
            
            if (image.sprite == null)
            {
                Debug.LogError("Sprite is null on " + gameObject.name);
                return null;
            }

            return GetAssetPath(image.sprite.texture);
        }
#endif
    }
}