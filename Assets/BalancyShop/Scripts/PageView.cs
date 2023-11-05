using System;
using Balancy.Addressables;
using Balancy.Models.BalancyShop;
using Balancy.Models.SmartObjects;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Page = Balancy.Models.LiveOps.Store.Page;

namespace BalancyShop
{
    public class PageView : MonoBehaviour
    {
        [SerializeField] private TMP_Text header;
        [SerializeField] private ContentHolder content;
        [SerializeField] private GameObject slotPrefab;

        private int pageRefreshIndex = 0;

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

            var pageIndex = ++pageRefreshIndex;
            PreloadPrefabs(page, () =>
            {
                if (pageIndex != pageRefreshIndex)
                    return;
                
                foreach (var storeSlot in page.ActiveSlots)
                {
                    if (storeSlot is MyCustomSlot myCustomSlot)
                    {
                        var ui = myCustomSlot.UIData;
                        AssetsLoader.GetObject(ui.Asset.Name, prefab =>
                        {
                            if (content == null || UnityObjectUtility.IsDestroyed(content) || pageIndex != pageRefreshIndex)
                                return;
                            
                            var storeItemView = content.AddElement<SlotView>(prefab as GameObject);
                            storeItemView.Init(storeSlot);
                        });
                    }
                    else
                    {
                        var storeItemView = content.AddElement<SlotView>(slotPrefab);
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
