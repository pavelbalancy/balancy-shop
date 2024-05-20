using Balancy.Models;
using Balancy.Models.BalancyShop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BalancyShop
{
    public class BattlePointView : MonoBehaviour
    {
        [SerializeField] private TMP_Text freeCount;
        [SerializeField] private TMP_Text goldCount;
        
        [SerializeField] private Image freeIcon;
        [SerializeField] private Image goldIcon;
        
        [SerializeField] private TMP_Text level;

        public void Init(BattlePassPoint point)
        {
            if (point.FreeReward.Items.Length > 0)
            {
                var item = point.FreeReward.Items[0];
                freeCount.text = item?.Count.ToString();

                (item.Item as MyItem)?.Icon?.LoadSprite(sprite =>
                {
                    if (freeIcon != null)
                        freeIcon.sprite = sprite;
                });
            }
            
            if (point.PremiumReward.Items.Length > 0)
            {
                var item = point.PremiumReward.Items[0];
                goldCount.text = item?.Count.ToString();

                (item.Item as MyItem)?.Icon?.LoadSprite(sprite =>
                {
                    if (goldIcon != null)
                        goldIcon.sprite = sprite;
                });
            }

            level.text = point.Scores.ToString();
        }
    }
}
