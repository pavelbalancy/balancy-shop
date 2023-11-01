using System;
using Balancy;
using Balancy.Addressables;
using Balancy.API.Payments;
using Balancy.Data.SmartObjects;
using Balancy.Models.BalancyShop;
using Balancy.Models.LiveOps.Store;
using Balancy.Models.SmartObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BalancyShop
{
    public class BuyButton : MonoBehaviour
    {
        private interface IBuyButton
        {
            void TryToBuy();
            StoreItem GetStoreItem();
            bool IsAvailable();
            UIStoreItem GetUIData();
        }

        private class BuyButtonStoreSlot : IBuyButton
        {
            private readonly Slot _slot;

            public BuyButtonStoreSlot(Slot slot)
            {
                _slot = slot;
            }
            
            public void TryToBuy()
            {
                var storeItem = _slot.GetStoreItem();
                if (storeItem.IsFree())
                {
                    Balancy.LiveOps.Store.ItemWasPurchased(storeItem, null);
                }
                else
                {
                    switch (storeItem.Price.Type)
                    {
                        case PriceType.Hard:
                            //TODO write your payment logic here and then invoke the following method:
                            Balancy.LiveOps.Store.ItemWasPurchased(storeItem, new PaymentInfo
                            {
                                Currency = "USD",
                                Price = storeItem.Price.Product.Price,
                                ProductId = storeItem.Price.Product.ProductId
                            }, response =>
                            {
                                Debug.Log("Purchase " + response.Success + " ? " + response.Error?.Message);
                                //TODO give resources from Reward
                            });
                            break;
                        case PriceType.Soft:
                            //TODO Try to take soft resources from Price
                            Balancy.LiveOps.Store.ItemWasPurchased(storeItem, storeItem.Price);
                            //TODO give resources from Reward
                            break;
                        case PriceType.Ads:
                            if (storeItem.IsEnoughResources())
                            {
                                Balancy.LiveOps.Store.PurchaseStoreItem(storeItem,
                                    response =>
                                    {
                                        Debug.Log("Store item was purchased for Ads: " + response.Success);
                                    });
                            }
                            else
                            {
                                LiveOps.Store.AdWasWatchedForStoreItem(storeItem);
                            }

                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            public StoreItem GetStoreItem()
            {
                return _slot.GetStoreItem();
            }

            public bool IsAvailable()
            {
                return _slot.IsAvailable();
            }

            public UIStoreItem GetUIData()
            {
                return (_slot as MyCustomSlot)?.UIData;
            }
        }
        
        private class BuyButtonOffer : IBuyButton
        {
            private readonly OfferInfo _offerInfo;

            public BuyButtonOffer(OfferInfo offerInfo)
            {
                _offerInfo = offerInfo;
            }
            
            public void TryToBuy()
            {
                var storeItem = _offerInfo.GameOffer.StoreItem;
                if (storeItem.IsFree())
                {
                    Balancy.LiveOps.GameOffers.OfferWasPurchased(_offerInfo, null);
                }
                else
                {
                    switch (storeItem.Price.Type)
                    {
                        case PriceType.Hard:
                            //TODO write your payment logic here and then invoke the following method:
                            Balancy.LiveOps.GameOffers.OfferWasPurchased(_offerInfo, new PaymentInfo
                            {
                                Currency = "USD",
                                Price = storeItem.Price.Product.Price,
                                ProductId = storeItem.Price.Product.ProductId
                            }, response =>
                            {
                                Debug.Log("Purchase " + response.Success + " ? " + response.Error?.Message);
                                //TODO give resources from Reward
                            });
                            break;
                        case PriceType.Soft:
                            //TODO Try to take soft resources from Price
                            Balancy.LiveOps.GameOffers.OfferWasPurchased(_offerInfo, storeItem.Price);
                            //TODO give resources from Reward
                            break;
                        case PriceType.Ads:
                            if (storeItem.IsEnoughResources())
                            {
                                Balancy.LiveOps.GameOffers.PurchaseOffer(_offerInfo,
                                    response =>
                                    {
                                        Debug.Log("Store item was purchased for Ads: " + response.Success);
                                    });
                            }
                            else
                            {
                                LiveOps.Store.AdWasWatchedForStoreItem(storeItem);
                            }

                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            public StoreItem GetStoreItem()
            {
                return _offerInfo.GameOffer.StoreItem;
            }

            public bool IsAvailable()
            {
                return true;
            }

            public UIStoreItem GetUIData()
            {
                return (_offerInfo.GameOffer as MyOffer)?.UIStoreSlotData;
            }
        }
        
        [SerializeField] private Button buyButton;
        [SerializeField] private Image buyIcon;
        [SerializeField] private TMP_Text buyText;
        [SerializeField] private TMP_Text buyHintText;
        [SerializeField] private Image buyButtonBack;
        
        [SerializeField] private Sprite watchAdIcon;
        
        private IBuyButton _buyButtonLogic;
        
        public void Init(Slot slot)
        {
            _buyButtonLogic = new BuyButtonStoreSlot(slot);
            Refresh();
        }
        
        public void Init(OfferInfo offerInfo)
        {
            _buyButtonLogic = new BuyButtonOffer(offerInfo);
            Refresh();
        }
        
        private void Start()
        {
            buyButton?.onClick.AddListener(TryToBuy);
        }

        private void TryToBuy()
        {
            _buyButtonLogic.TryToBuy();
            Refresh();
        }

        public void Refresh()
        {
            ApplyBuyButton(_buyButtonLogic.GetStoreItem());
        }
        
        private void ApplyBuyButton(StoreItem storeItem)
        {
            buyIcon?.gameObject.SetActive(false);
            string buyTextString = "WRONG_PRICE";
            if (storeItem.IsFree())
            {
                buyTextString = "Free";
            }
            else
            {
                switch (storeItem.Price.Type)
                {
                    case PriceType.Hard:
                        buyTextString = "$ " + storeItem.Price.Product?.Price;
                        break;
                    case PriceType.Soft:
                        var firstItem = storeItem.Price.Items.Length > 0 ? storeItem.Price.Items[0] : null;
                        if (firstItem == null)
                        {
                            buyTextString = "No price";
                        }
                        else
                        {
                            buyTextString = firstItem.Count.ToString();
                            if (buyIcon != null)
                            {
                                (firstItem.Item as MyItem)?.Icon.LoadSprite(sprite =>
                                {
                                    buyIcon.gameObject.SetActive(true);
                                    buyIcon.sprite = sprite;
                                });
                            }
                        }
                        break;
                    case PriceType.Ads:
                        if (buyIcon != null)
                        {
                            buyIcon.gameObject.SetActive(true);
                            buyIcon.sprite = watchAdIcon;
                        }

                        if (storeItem.IsEnoughResources())
                            buyTextString = "Collect";
                        else
                            buyTextString = $"{storeItem.GetWatchedAds()}/{storeItem.GetRequiredAdsToWatch()}";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            bool showHint = false;
            // switch (_slot)
            // {
            //     case SlotPeriod slotPeriod:
            //     {
            //         buyTextString += $" ({slotPeriod.GetPurchasesDoneCount()}/{slotPeriod.Limit})";
            //         buyHintText.SetText($"Resets in {slotPeriod.GetSecondsUntilReset()}");//TODO fix
            //         showHint = true;
            //         break;
            //     }
            //     case SlotCooldown slotCooldown:
            //     {
            //         if (slotCooldown.IsAvailable())
            //             buyHintText.SetText($"CD {slotCooldown.Cooldown}");
            //         else
            //             buyHintText.SetText($"Resets in {slotCooldown.GetSecondsLeftUntilAvailable()}");
            //         showHint = true;
            //         break;
            //     }
            // }
            
            buyText?.SetText(buyTextString);
            buyHintText?.gameObject.SetActive(showHint);
            if (buyButton != null)
                buyButton.interactable = _buyButtonLogic.IsAvailable();

            if (buyButtonBack != null)
            {
                UIStoreItem uiStoreItem = _buyButtonLogic.GetUIData();
                if (uiStoreItem?.Button != null)
                    uiStoreItem.Button.LoadSprite(btnSprite => { buyButtonBack.sprite = btnSprite; });
            }
        }
    }
}
