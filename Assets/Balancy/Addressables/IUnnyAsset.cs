namespace Balancy.Addressables
{
    public interface IUnnyAsset
    {
#if UNITY_EDITOR
        string GetPreviewImagePath();
#endif
    }
}