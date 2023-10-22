using System;
using Balancy.Addressables;
using Balancy.Models.BalancyShop;
using Balancy.Models.LiveOps.Store;
using Balancy.Models.SmartObjects;
using TMPro;
using UnityEngine;

namespace BalancyShop
{
    public class BalancyShopPageView : MonoBehaviour
    {
        [SerializeField] private TMP_Text header;
        [SerializeField] private ContentHolder content;
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
            content.CleanUp();

            PreloadPrefabs(page, () =>
            {
                foreach (var storeSlot in page.ActiveSlots)
                {
                    if (storeSlot is MyCustomSlot myCustomSlot)
                    {
                        var ui = myCustomSlot.UIData;
                        AssetsLoader.GetObject(ui.Asset.Name, prefab =>
                        {
                            var storeItemView = content.AddElement<BalancyShopSlotView>(prefab as GameObject);
                            storeItemView.Init(storeSlot);
                        });
                    }
                    else
                    {
                        var storeItemView = content.AddElement<BalancyShopSlotView>(slotPrefab);
                        storeItemView.Init(storeSlot);
                    }
                }
            });
        }

        private void PreloadPrefabs(Page page, Action callback)
        {
            var loadingElements = page.ActiveSlots.Count;
            foreach (var storeSlot in page.ActiveSlots)
            {
                if (storeSlot is MyCustomSlot myCustomSlot)
                {
                    var ui = myCustomSlot.UIData;
                    AssetsLoader.GetObject(ui.Asset.Name, prefab =>
                    {
                        loadingElements--;
                        if (loadingElements == 0)
                            callback?.Invoke();
                    });
                } else
                {
                    loadingElements--;
                    if (loadingElements == 0)
                        callback?.Invoke();
                }
            }
        }
    }
}
