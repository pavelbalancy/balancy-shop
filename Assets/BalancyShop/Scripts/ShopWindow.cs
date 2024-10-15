using Balancy;
using Balancy.Data;
using Balancy.Example;
using Balancy.Models.SmartObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BalancyShop
{
    public class ShopWindow : BaseWindow
    {
        [SerializeField] private TMP_Text header;
        [SerializeField] private RectTransform content;
        
        [SerializeField] private GameObject pagePrefab;
        [SerializeField] private GameObject pageOffersPrefab;
        [SerializeField] private InventoryView inventoryResources;

        private GameStoreBase _smartConfig;

        public override void Refresh()
        {
            base.Refresh();
            if (LiveOps.Store.DefaultStore != null)
            {
                Refresh(LiveOps.Store.DefaultStore);
                inventoryResources?.Init(LiveOps.Profile.Inventories.Currencies);
            }
        }

        public override void SetActive(bool isActive)
        {
            base.SetActive(isActive);
            if (LiveOps.Store.DefaultStore != null)
                SetShopOpenStatus();
        }

        private void SetShopOpenStatus()
        {
            Balancy.Data.SmartStorage.LoadSmartObject<BalancyShopData>(response =>
            {
                var shopData = response.Data;
                shopData.Info.ShopOpened = gameObject.activeSelf;
            });
        }

        private void OnDestroy()
        {
            CleanUp();
        }

        private void CleanUp()
        {
            if (_smartConfig != null)
            {
                GameStoreBase.OnStoreUpdatedEvent -= Refresh;
                _smartConfig = null;
            }
        }

        public void Refresh(GameStoreBase smartConfig)
        {
            SetShopOpenStatus();
            
            if (_smartConfig != smartConfig)
            {
                CleanUp();
                _smartConfig = smartConfig;
                GameStoreBase.OnStoreUpdatedEvent += Refresh;
            }
            
            content.RemoveChildren();

            AddOffersSection();
            
            var pages = smartConfig.ActivePages;
            foreach (var page in pages)
            {
                var newButton = Instantiate(pagePrefab, content);
                var storeTabButton = newButton.GetComponent<PageView>();
                storeTabButton.Init(smartConfig, page, RefreshSize);
            }
        }

        private Coroutine _coroutine;
        private void RefreshSize()
        {
            if (_coroutine != null)
                Coroutines.StopCoroutineRemotely(_coroutine);
            
            _coroutine = Coroutines.WaitTwoFrames(() =>
            {
                _coroutine = null;
                LayoutRebuilder.ForceRebuildLayoutImmediate(content);
            });
        }

        private void AddOffersSection()
        {
            var newButton = Instantiate(pageOffersPrefab, content);
            var storeTabButton = newButton.GetComponent<PageViewOffers>();
            storeTabButton.Init();
        }
    }
}
