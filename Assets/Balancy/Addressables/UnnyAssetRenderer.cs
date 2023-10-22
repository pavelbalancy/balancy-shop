using UnityEngine;

namespace Balancy.Addressables
{
    public class UnnyAssetRenderer : UnnyAssetPath, IUnnyAsset
    {
#if UNITY_EDITOR
        public string GetPreviewImagePath()
        {
            var renderer = GetComponentInChildren<Renderer>();
            if (renderer == null)
            {
                Debug.LogError("Renderer wasn't found on " + gameObject.name);
                return null;
            }

            var allTextures = Balancy.EditorUtils.GetAllTexturesFromMaterial(renderer);

            foreach (var texture in allTextures)
            {
                if (texture is Texture2D)
                    return GetAssetPath(texture);
            }

            Debug.LogError("Texture2D wasn't found in material of " + gameObject.name);
            return null;
        }
#endif
    }
}