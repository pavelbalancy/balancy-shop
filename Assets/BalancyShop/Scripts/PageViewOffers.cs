using System;
using Balancy;
using Balancy.Addressables;
using Balancy.Data.SmartObjects;
using Balancy.Models.BalancyShop;
using Balancy.Models.SmartObjects;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Page = Balancy.Models.LiveOps.Store.Page;

namespace BalancyShop
{
    public class PageViewOffers : MonoBehaviour
    {
        [SerializeField] private TMP_Text header;
        [SerializeField] private ContentHolder content;
        [SerializeField] private GameObject slotPrefab;

        public void Init()
        {
            BalancyShopSmartObjectsEvents.onOfferActivated += OnOfferActivated;
            BalancyShopSmartObjectsEvents.onOfferDeactivated += OnOfferDeactivated;
            Refresh();
            
            header?.SetText("Limited Offers");
        }

        private void OnOfferActivated(OfferInfo offerInfo)
        {
            Refresh();
        }

        private void OnOfferDeactivated(OfferInfo offerInfo)
        {
            Refresh();
        }

        private void OnDestroy()
        {
            BalancyShopSmartObjectsEvents.onOfferActivated -= OnOfferActivated;
            BalancyShopSmartObjectsEvents.onOfferDeactivated -= OnOfferDeactivated;
        }

        private void Refresh()
        {
            content.CleanUp();

            var allOffers = LiveOps.GameOffers.GetActiveOffers();

            if (allOffers.Length == 0)
            {
                gameObject.SetActive(false);
                return;
            } else
                gameObject.SetActive(true);
            
            PreloadPrefabs(allOffers, () =>
            {
                foreach (var offerInfo in allOffers)
                {
                    if (offerInfo.GameOffer is MyOffer myOffer)
                    {
                        var ui = myOffer.UIStoreSlotData;
                        if (ui?.Asset?.Name != null)
                        {
                            AssetsLoader.GetObject(ui.Asset.Name, prefab =>
                            {
                                if (content == null || UnityObjectUtility.IsDestroyed(content))
                                    return;
                            
                                var storeItemView = content.AddElement<SlotView>(prefab as GameObject);
                                storeItemView.Init(offerInfo, myOffer.UIStoreSlotData);
                            });
                        }
                    }
                }
            });
        }

        private void PreloadPrefabs(OfferInfo[] allOffers, Action callback)
        {
            var loadingElements = allOffers.Length;
            foreach (var offerInfo in allOffers)
            {
                if (offerInfo.GameOffer is MyOffer myOffer)
                {
                    var ui = myOffer.UIStoreSlotData;
                    if (ui?.Asset?.Name != null)
                    {
                        AssetsLoader.GetObject(ui.Asset.Name, prefab =>
                        {
                            loadingElements--;
                            if (loadingElements == 0)
                                callback?.Invoke();
                        });
                        continue;
                    }
                }

                loadingElements--;
                if (loadingElements == 0)
                    callback?.Invoke();
            }
        }
    }
}
