using System;
using Balancy;
using Balancy.Addressables;
using Balancy.Data.SmartObjects;
using Balancy.Example;
using Balancy.Models.BalancyShop;
using Balancy.Models.LiveOps.Store;
using Balancy.Models.SmartObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace BalancyShop
{
    public class SlotView : MonoBehaviour
    {
        private const int REFRESH_RATE = 1; 
        
        [SerializeField] private TMP_Text slotName;
        [SerializeField] private TMP_Text slotDescription;
        [SerializeField] private TMP_Text count;
        [SerializeField] private TMP_Text countCrossed;
        [SerializeField] private Image icon;
        
        [SerializeField] private GameObject ribbon;
        [SerializeField] private Image ribbonIcon;
        [SerializeField] private TMP_Text ribbonText;
        
        [SerializeField] private BuyButton buyButton;

        [SerializeField] private QualityRibbonConfig ribbonConfig;
        [SerializeField] private ForceTransformUpdater transformUpdater;
        
        [SerializeField] private Image background;
        [SerializeField] private ContentHolder rewardContent;
        [SerializeField] private GameObject defaultItemView;
        [SerializeField] private TMP_Text timer;

        private StoreItem _storeItem;
        private UIStoreItem _uiStoreItem;
        private bool _needToSubscribe;
        private bool _subscribed;

        private Func<int> onGetTimeLeft;

        public void Init(Slot slot)
        {
            _needToSubscribe = NeedToSubscribe(slot);
            Init(slot.GetStoreItem(), (slot as MyCustomSlot)?.UIData);
            ApplyRibbon(slot);
            buyButton.Init(slot);
        }
        
        private bool NeedToSubscribe(Slot slot)
        {
            switch (slot)
            {
                case SlotPeriod _:
                case SlotCooldown _:
                    return true;
            }

            return false;
        }

        public void Init(OfferInfo offerInfo)
        {
            _needToSubscribe = true;
            Init(offerInfo.GameOffer.StoreItem, (offerInfo.GameOffer as MyOffer)?.UIStoreSlotData);
            buyButton.Init(offerInfo);
            onGetTimeLeft = offerInfo.GetSecondsLeftBeforeDeactivation;
        }

        private void Init(StoreItem storeItem, UIStoreItem uiStoreItem)
        {
            _storeItem = storeItem;
            _uiStoreItem = uiStoreItem;
            ApplyBackground(_uiStoreItem);
            ApplyMainInfo(_storeItem);
            ShowReward(storeItem, uiStoreItem);
            SubscribeForTimers();
        }

        private void ApplyBackground(UIStoreItem uiStoreItem)
        {
            if (uiStoreItem != null)
            {
                AssetsLoader.GetSprite(uiStoreItem.Background, sprite =>
                {
                    background.sprite = sprite;
                });
            }
        }

        private void SubscribeForTimers()
        {
            if (_subscribed || !_needToSubscribe)
                return;

            _subscribed = true;
            BalancyTimer.SubscribeForTimer(REFRESH_RATE, Refresh);
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
            buyButton.Refresh();

            if (timer != null && onGetTimeLeft != null)
                timer.text = $"{onGetTimeLeft()} s";
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
            slotDescription?.SetText((storeItem as MyStoreItem)?.Description.Value);
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
            var config = ribbonConfig?.GetQualityConfig(slot.Type);
            if (config == null)
                ribbon?.SetActive(false);
            else
            {
                ribbonText?.SetText(config.DisplayText);
                if (ribbonIcon != null)
                    ribbonIcon.sprite = config.RibbonImage;
                ribbon.SetActive(true);
            }
        }

        private void ShowReward(StoreItem storeItem, UIStoreItem uiStoreItem)
        {
            if (rewardContent == null)
                return;
            
            rewardContent.CleanUp();
            
            PreloadPrefabs(uiStoreItem, () =>
            {
                var reward = storeItem.Reward;
                for (var i = 0; i < reward.Items.Length; i++)
                {
                    var itemWithAmount = reward.Items[i];
                    var uiItem = GetItemInfo(uiStoreItem, i);

                    void PrepareItemView(Object prefab)
                    {
                        var storeItemView = rewardContent.AddElement<ItemView>(prefab as GameObject);
                        storeItemView.Init(itemWithAmount, uiItem);
                    }
                    
                    if (uiItem != null)
                    {
                        if (uiItem.Asset == null)
                            PrepareItemView(defaultItemView);
                        else
                            AssetsLoader.GetObject(uiItem.Asset.Name, PrepareItemView);
                    }
                    else
                    {
                        PrepareItemView(defaultItemView);
                    }
                }
            });
        }

        private UIItem GetItemInfo(UIStoreItem uiStoreItem, int index)
        {
            if (uiStoreItem == null || uiStoreItem.Content.Length == 0)
                return null;

            if (uiStoreItem.Content.Length <= index)
                return uiStoreItem.Content[uiStoreItem.Content.Length - 1];
            return uiStoreItem.Content[index];
        }

        private void PreloadPrefabs(UIStoreItem uiStoreItem, Action callback)
        {
            if (uiStoreItem == null)
            {
                callback?.Invoke();
                return;
            }

            var loadingElements = uiStoreItem.Content.Length;
            if (loadingElements == 0)
                callback?.Invoke();
            else
            {
                foreach (var uiItem in uiStoreItem.Content)
                {
                    if (uiItem.Asset != null)
                    {
                        AssetsLoader.GetObject(uiItem.Asset.Name, prefab =>
                        {
                            loadingElements--;
                            if (loadingElements == 0)
                                callback?.Invoke();
                        });
                    }
                    else
                    {
                        loadingElements--;
                        if (loadingElements == 0)
                            callback?.Invoke();
                    }
                }
            }
        }
    }
}