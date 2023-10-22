using System.Collections.Generic;
using Balancy.Example;
using UnityEngine;

namespace BalancyShop
{
    public class ContentHolder : MonoBehaviour
    {
        [SerializeField] private GameObject element;
        [SerializeField] private bool isHorizontalLayout;
        
        private class RowInfo
        {
            public RectTransform Transform;
            public TakeSizeFromContent TakeSizeFromContent;
        }
        
        private List<RowInfo> children;
        public void CleanUp()
        {
            transform.RemoveChildren();
            children = new List<RowInfo>();
        }

        public T AddElement<T>(GameObject prefab) where T : MonoBehaviour
        {
            var row = GetActiveRowInfo(prefab);
            var newItem = Instantiate(prefab, row.Transform);
            row.TakeSizeFromContent.SetContent(newItem.transform as RectTransform);
            return newItem.GetComponent<T>();
        }

        private RowInfo GetActiveRowInfo(GameObject nextPrefab)
        {
            if (children.Count != 0)
            {
                var lastRow = children[children.Count - 1];
                var availableSpace = GetContentAvailableSize(lastRow.Transform);
                var nextTrm = (nextPrefab.transform as RectTransform);
                float requiredSpace = 0;
                if (nextTrm != null)
                    requiredSpace = isHorizontalLayout ? nextTrm.sizeDelta.x : nextTrm.sizeDelta.y;

                if (availableSpace > requiredSpace)
                    return lastRow;
            }

            RowInfo newRow = CreateNewRow();
            return newRow;
        }

        private float GetContentAvailableSize(RectTransform rectTransform)
        {
            float size = 0;
            for (int i = 0; i < rectTransform.childCount; i++)
            {
                var child = rectTransform.GetChild(i) as RectTransform;
                if (child == null)
                    continue;
                
                size += isHorizontalLayout ? child.sizeDelta.x : child.sizeDelta.y;
            }
            
            return (isHorizontalLayout ? rectTransform.sizeDelta.x : rectTransform.sizeDelta.y) - size;
        }

        private RowInfo CreateNewRow()
        {
            var newRow = Instantiate(element, transform);
            newRow.name = $"Element-{children.Count + 1}";
            var newRowTrm = newRow.transform as RectTransform;
            var rowInfo = new RowInfo
            {
                Transform = newRowTrm,
                TakeSizeFromContent = newRow.GetComponent<TakeSizeFromContent>()
            };
            children.Add(rowInfo);
            return rowInfo;
        }
    }
}
