using UnityEngine;

namespace Balancy.Addressables
{
    public class UnnyAssetSpriteRenderer : UnnyAssetPath, IUnnyAsset
    {
#if UNITY_EDITOR
        public string GetPreviewImagePath()
        {
            var spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                Debug.LogError("SpriteRenderer wasn't found on " + gameObject.name);
                return null;
            }
            
            if (spriteRenderer.sprite == null)
            {
                Debug.LogError("Sprite is null on " + gameObject.name);
                return null;
            }

            return GetAssetPath(spriteRenderer.sprite.texture);
        }
#endif
    }
}