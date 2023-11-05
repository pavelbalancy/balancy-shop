using Balancy;
using Balancy.Localization;
using Balancy.Models.BalancyShop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BalancyShop
{
    public class SaleView : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private Image image;

        public void SetMultiplier(float multiplier)
        {
            if (multiplier > 1f)
            {
                gameObject.SetActive(true);
                text.SetText(string.Format(Manager.Get("DEFAULT/SHOP_SALE"), Utils.ToInt(multiplier)));
            }
            else
                gameObject.SetActive(false);
        }

        public void SetBadge(BadgeInfo badgeInfo)
        {
            gameObject.SetActive(true);
            text.SetText(badgeInfo.Text.Value);
        }
    }
}