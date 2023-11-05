using System;
using Balancy;
using Balancy.Models.BalancyShop;
using Balancy.Models.GameShop;
using Balancy.Models.LiveOps.Store;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BalancyShop
{
    public class ControlButton : MonoBehaviour
    {
        private const int REFRESH_RATE = 1; 
        
        [SerializeField] private TMP_Text text;
        [SerializeField] private Image back;
        [SerializeField] private Image icon;
        [SerializeField] private Button button;
        
        [SerializeField] private GameObject badge;
        [SerializeField] private Image badgeBack;
        [SerializeField] private TMP_Text badgeText;

        private GameSection _section;
        private Action<GameSection> _onClick;
        private BadgeInfo _badgeInfo;
        
        public void Init(GameSection section, Action<GameSection> onClick)
        {
            _section = section;
            _onClick = onClick;
            
            text.SetText(section.Text.Value);

            section.DefaultBack.LoadSprite(sprite => back.sprite = sprite);
            section.Icon.LoadSprite(sprite => icon.sprite = sprite);
            
            button.onClick.AddListener(OnClicked);

            if (_section.Type == WindowType.Shop)
                CheckShopBadges();
            else
                ApplyBadge();
        }

        private void CheckShopBadges()
        {
            SetTimer(true);
            ApplyShopBadge();
        }
        
        private bool _subscribedForTimer;

        private void SetTimer(bool required)
        {
            if (required == _subscribedForTimer)
                return;

            if (required)
                BalancyTimer.SubscribeForTimer(REFRESH_RATE, ApplyShopBadge);
            else
                BalancyTimer.UnsubscribeFromTimer(REFRESH_RATE, ApplyShopBadge);

            _subscribedForTimer = required;
        }

        private void ApplyShopBadge()
        {
            var badgeInfo = FindBadgeInfoWithHighestPriority();

            if (_badgeInfo == badgeInfo)
                return;

            if (badgeInfo != null)
            {
                badgeInfo.ShopButtonIcon?.LoadSprite(sprite => badgeBack.sprite = sprite);
                badgeText.SetText(badgeInfo.Text.Value.ToUpper());
                badge.SetActive(true);
            } else
                ApplyBadge();

            _badgeInfo = badgeInfo;
        }

        private BadgeInfo FindBadgeInfoWithHighestPriority()
        {
            var shop = LiveOps.Store.DefaultStore;
            if (shop == null)
                return null;
            
            SlotType type = SlotType.Default;
            int priority = 0;
            foreach (var page in shop.ActivePages)
            {
                foreach (var slot in page.ActiveSlots)
                {
                    if (slot.IsAvailable() && slot.Type != SlotType.Default && slot.GetStoreItem() is MyStoreItem myStoreItem)
                    {
                        if (myStoreItem.BadgePriority > priority)
                        {
                            priority = myStoreItem.BadgePriority;
                            type = slot.Type;
                        }
                    }
                }
            }

            var offers = LiveOps.GameOffers.GetActiveOffers();
            foreach (var offer in offers)
            {
                if (offer.GameOffer is MyOffer myOffer)
                {
                    if (myOffer.Badge != SlotType.Default && myOffer.StoreItem is MyStoreItem myStoreItem)
                    {
                        if (myStoreItem.BadgePriority > priority)
                        {
                            priority = myStoreItem.BadgePriority;
                            type = myOffer.Badge;
                        }
                    }
                }
            }

            if (type != SlotType.Default)
                return GameUtils.FindBadgeInfo(type);

            return null;
        }

        private void ApplyBadge()
        {
            if (_section.Badge == null)
            {
                badge.SetActive(false);
                return;
            }

            _section.Badge.Back.LoadSprite(sprite => badgeBack.sprite = sprite);
            badgeText.SetText(_section.Badge.Text.Value.ToUpper());
            badge.SetActive(true);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveAllListeners();
            SetTimer(false);
        }

        private void OnClicked()
        {
            _onClick?.Invoke(_section);
        }

        public void SetSelected(GameSection section)
        {
            SetSelected(_section == section);
        }

        private void SetSelected(bool selected)
        {
            var background = selected ? _section.SelectedBack : _section.DefaultBack;
            background.LoadSprite(sprite => back.sprite = sprite);
        }
    }
}