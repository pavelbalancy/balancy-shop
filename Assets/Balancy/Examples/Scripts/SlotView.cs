using System;
using Balancy.API.Payments;
using Balancy.Models.LiveOps.Store;
using Balancy.Models.SmartObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Balancy.Example
{
    public class SlotView : MonoBehaviour
    {
        private const int REFRESH_RATE = 1; 
        
        [SerializeField] private TMP_Text slotName;
        [SerializeField] private TMP_Text count;
        [SerializeField] private TMP_Text countCrossed;
        [SerializeField] private Image icon;
        
        [SerializeField] private GameObject ribbon;
        [SerializeField] private Image ribbonIcon;
        [SerializeField] private TMP_Text ribbonText;
        
        [SerializeField] private Button buyButton;
        [SerializeField] private Image buyIcon;
        [SerializeField] private TMP_Text buyText;
        [SerializeField] private TMP_Text buyHintText;

        [SerializeField] private QualityRibbonConfig ribbonConfig;
        [SerializeField] private ForceTransformUpdater transformUpdater;

        private Slot _slot;
        private bool _subscribed;

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

        public void Init(Slot slot)
        {
            _slot = slot;
            ApplyMainInfo(slot.GetStoreItem());
            ApplyRibbon(slot);
            ApplyBuyButton(slot.GetStoreItem());
            SubscribeForTimers();
        }

        private void SubscribeForTimers()
        {
            if (_subscribed || _slot == null)
                return;
            
            switch (_slot)
            {
                case SlotPeriod _:
                case SlotCooldown _:
                {
                    _subscribed = true;
                    BalancyTimer.SubscribeForTimer(REFRESH_RATE, Refresh);
                    break;
                } 
            }
        }

        private void UnsubscribeFromTimers()
        {
            if (!_subscribed)
                return;

            _subscribed = false;
            BalancyTimer.UnsubscribeFromTimer(REFRESH_RATE, Refresh);
        }

        private void Refresh()
        {
            ApplyBuyButton(_slot.GetStoreItem());
        }

        private void OnEnable()
        {
            SubscribeForTimers();
        }

        private void OnDisable()
        {
            UnsubscribeFromTimers();
        }

        private void ApplyMainInfo(StoreItem storeItem)
        {
            slotName?.SetText(storeItem.Name.Value);
            if (icon != null)
                storeItem.Sprite?.LoadSprite(sprite =>
                {
                    icon.sprite = sprite;
                });

            ApplyResourcesCount(storeItem);
        }

        private void ApplyResourcesCount(StoreItem storeItem)
        {
            var firstItem = storeItem.Reward.Items.Length > 0 ? storeItem.Reward.Items[0] : null;
            if (firstItem != null)
            {
                count?.SetText(firstItem.Count.ToString());
                if (storeItem.GetMultiplier() <= 1.001f)
                {
                    countCrossed?.gameObject.SetActive(false);
                }
                else
                {
                    var originalCount = (int)(firstItem.Count / storeItem.GetMultiplier() + 0.5f);
                    countCrossed?.SetText(originalCount.ToString());
                    countCrossed?.gameObject.SetActive(true);
                }
                
                transformUpdater?.ForceUpdate();
            }
            else
            {
                count?.gameObject.SetActive(false);
                countCrossed?.gameObject.SetActive(false);
            }
        }

        private void ApplyRibbon(Slot slot)
        {
            var config = ribbonConfig.GetQualityConfig(slot.Type);
            if (config == null)
                ribbon.SetActive(false);
            else
            {
                ribbonText?.SetText(config.DisplayText);
                if (ribbonIcon != null)
                    ribbonIcon.sprite = config.RibbonImage;
                ribbon.SetActive(true);
            }
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
                                //TODO buyIcon.sprite = 
                            }
                        }
                        break;
                    case PriceType.Ads:
                        buyTextString = $"Watch Ads: {storeItem.GetWatchedAds()}/{storeItem.GetRequiredAdsToWatch()}";
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
        }
    }
}