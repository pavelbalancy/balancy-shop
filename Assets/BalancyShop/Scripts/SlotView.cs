using System;
using Balancy;
using Balancy.Addressables;
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

        private Slot _slot;
        private bool _subscribed;

        public void Init(Slot slot)
        {
            _slot = slot;
            ApplyBackground(slot);
            ApplyMainInfo(slot.GetStoreItem());
            ApplyRibbon(slot);
            buyButton.Init(slot);
            ShowReward();
            SubscribeForTimers();
        }

        private void ApplyBackground(Slot slot)
        {
            if (slot is MyCustomSlot myCustomSlot)
            {
                AssetsLoader.GetSprite(myCustomSlot.UIData.Background, sprite =>
                {
                    background.sprite = sprite;
                });
            }
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
            buyButton.Refresh();
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

        private void ShowReward()
        {
            if (rewardContent == null)
                return;
            
            rewardContent.CleanUp();
            
            PreloadPrefabs(() =>
            {
                var reward = _slot.GetStoreItem().Reward;
                for (var i = 0; i < reward.Items.Length; i++)
                {
                    var itemWithAmount = reward.Items[i];
                    var uiItem = GetItemInfo(i);

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

        private UIItem GetItemInfo(int index)
        {
            UIStoreItem uiStoreItem = (_slot as MyCustomSlot)?.UIData;

            if (uiStoreItem == null || uiStoreItem.Content.Length == 0)
                return null;

            if (uiStoreItem.Content.Length <= index)
                return uiStoreItem.Content[uiStoreItem.Content.Length - 1];
            return uiStoreItem.Content[index];
        }

        private void PreloadPrefabs(Action callback)
        {
            UIStoreItem uiStoreItem = (_slot as MyCustomSlot)?.UIData;
            
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