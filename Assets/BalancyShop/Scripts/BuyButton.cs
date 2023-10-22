using System;
using Balancy;
using Balancy.Addressables;
using Balancy.API.Payments;
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
        [SerializeField] private Button buyButton;
        [SerializeField] private Image buyIcon;
        [SerializeField] private TMP_Text buyText;
        [SerializeField] private TMP_Text buyHintText;
        [SerializeField] private Image buyButtonBack;
        
        [SerializeField] private Sprite watchAdIcon;
        
        private Slot _slot;
        
        public void Init(Slot slot)
        {
            _slot = slot;
            ApplyBuyButton(slot.GetStoreItem());
        }
        
        private void Start()
        {
            buyButton?.onClick.AddListener(TryToBuy);
        }

        private void TryToBuy()
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
                                response => { Debug.Log("Store item was purchased for Ads: " + response.Success); });
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

            ApplyBuyButton(storeItem);
        }

        public void Refresh()
        {
            ApplyBuyButton(_slot.GetStoreItem());
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
            switch (_slot)
            {
                case SlotPeriod slotPeriod:
                {
                    buyTextString += $" ({slotPeriod.GetPurchasesDoneCount()}/{slotPeriod.Limit})";
                    buyHintText.SetText($"Resets in {slotPeriod.GetSecondsUntilReset()}");//TODO fix
                    showHint = true;
                    break;
                }
                case SlotCooldown slotCooldown:
                {
                    if (slotCooldown.IsAvailable())
                        buyHintText.SetText($"CD {slotCooldown.Cooldown}");
                    else
                        buyHintText.SetText($"Resets in {slotCooldown.GetSecondsLeftUntilAvailable()}");
                    showHint = true;
                    break;
                }
            }
            
            buyText?.SetText(buyTextString);
            buyHintText?.gameObject.SetActive(showHint);
            if (buyButton != null)
                buyButton.interactable = _slot.IsAvailable();

            if (_slot is MyCustomSlot myCustomSlot && buyButtonBack != null)
            {
                UIStoreItem uiStoreItem = myCustomSlot.UIData;
                AssetsLoader.GetSprite(uiStoreItem.Button, btnSprite =>
                {
                    buyButtonBack.sprite = btnSprite;
                });
            }
        }
    }
}
