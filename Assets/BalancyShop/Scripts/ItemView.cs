using Balancy.Models.BalancyShop;
using Balancy.Models.SmartObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BalancyShop
{
    public class ItemView : MonoBehaviour
    {
        [SerializeField] private Image background;
        [SerializeField] private TMP_Text text;
        [SerializeField] private Image icon;

        public void Init(ItemWithAmount itemWithAmount, UIItem overrideInfo)
        {
            ApplyIcon(itemWithAmount, overrideInfo);
            ApplyBackground(overrideInfo);
            ApplyText(itemWithAmount, overrideInfo);
        }

        private void ApplyIcon(ItemWithAmount itemWithAmount, UIItem overrideInfo)
        {
            if (overrideInfo?.Icon != null)
            {
                overrideInfo.Icon.LoadSprite(sprite => { icon.sprite = sprite; });
            }
            else if (itemWithAmount.Item is MyItem myItem)
            {
                myItem.Icon.LoadSprite(sprite => { icon.sprite = sprite; });
            }
        }
        
        private void ApplyBackground(UIItem overrideInfo)
        {
            overrideInfo?.Background?.LoadSprite(sprite => { background.sprite = sprite; });
        }

        private void ApplyText(ItemWithAmount itemWithAmount, UIItem overrideInfo)
        {
            if (overrideInfo?.Text.HasValue ?? false)
                text.SetText(string.Format(overrideInfo.Text.Value, itemWithAmount.Count));
            else
                text.SetText(itemWithAmount.Count.ToString());
        }
    }
}