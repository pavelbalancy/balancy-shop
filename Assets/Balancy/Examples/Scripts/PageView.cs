using Balancy.Models.LiveOps.Store;
using Balancy.Models.SmartObjects;
using TMPro;
using UnityEngine;

namespace Balancy.Example
{
    public class PageView : MonoBehaviour
    {
        [SerializeField] private TMP_Text header;
        [SerializeField] private RectTransform content;
        [SerializeField] private GameObject slotPrefab;

        private Page _page;
        public void Init(GameStoreBase smartConfig, Page page)
        {
            _page = page;
            page.OnStorePageUpdatedEvent += Refresh;
            Refresh(smartConfig, page);
            
            header?.SetText(page.Name.Value);
        }

        private void OnDestroy()
        {
            if (_page != null)
            {
                _page.OnStorePageUpdatedEvent -= Refresh;
                _page = null;
            }
        }

        private void Refresh(GameStoreBase smartConfig, Page page)
        {
            content.RemoveChildren();
            
            foreach (var storeSlot in page.ActiveSlots)
            {
                var newItem = Instantiate(slotPrefab, content);
                var storeItemView = newItem.GetComponent<SlotView>();
                storeItemView.Init(storeSlot);
            }
        }
    }
}
