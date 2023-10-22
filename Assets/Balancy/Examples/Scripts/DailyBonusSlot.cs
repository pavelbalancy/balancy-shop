using Balancy.Models.SmartObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Balancy.Example
{
    public class DailyBonusSlot : MonoBehaviour
    {
        [SerializeField] private TMP_Text dayNumber;
        [SerializeField] private TMP_Text itemNameAndCount;
        [SerializeField] private Image icon;
        
        [SerializeField] private Button claimButton;
        [SerializeField] private TMP_Text buttonTitle;

        private int _index;
        
        private void Start()
        {
            claimButton?.onClick.AddListener(TryToClaim);
            LiveOpsEventsExample.OnDailyRewardAvailableEvent += Refresh;
        }

        private void OnDestroy()
        {
            LiveOpsEventsExample.OnDailyRewardAvailableEvent -= Refresh;
        }

        private void Refresh(Reward reward, int dayNumber)
        {
            Refresh();
        }

        private void TryToClaim()
        {
            LiveOps.DailyBonus.ClaimNextReward();
            Refresh();
        }

        public void Init(int index, LiveOps.DailyBonus.RewardInfo slotRewardInfo)
        {
            _index = index;
            
            dayNumber.SetText($"Day {index + 1}");
            var firstItem = GetFirstItem(slotRewardInfo);
            if (firstItem != null)
            {
                itemNameAndCount.SetText($"{firstItem.Item.Name.Value} x{firstItem.Count}");
                //Apply Item
                // (firstItem.Item as GameItem)?.Icon?.LoadSprite(sprite =>
                // {
                //     icon.sprite = sprite;
                // });
            }

            Refresh();
        }

        private void Refresh()
        {
            if (claimButton != null)
                claimButton.interactable = LiveOps.DailyBonus.GetNextRewardNumber() == (_index + 1) &&
                                           LiveOps.DailyBonus.CanClaimNextReward();

            buttonTitle?.SetText(LiveOps.DailyBonus.GetAllRewards()[_index].Status.ToString());
        }

        private ItemWithAmount GetFirstItem(LiveOps.DailyBonus.RewardInfo slotRewardInfo)
        {
            var items = slotRewardInfo.Reward.Items;
            return items.Length > 0 ? items[0] : null;
        }
    }
}
