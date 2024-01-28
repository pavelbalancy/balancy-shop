using Balancy.Data.SmartObjects;
using Balancy.Models.BalancyShop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BalancyShop
{
    public class InventorySlotView : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private TMP_Text count;

        public void UpdateData(InventorySlot slot)
        {
            SetVisible(false);
            var item = slot.HasItem() ? slot.Item?.Item : slot.AcceptableItem;
            (item as MyItem)?.Icon.LoadSprite(sprite =>
            {
                if (sprite != null)
                {
                    image.sprite = sprite;
                    var amount = slot.HasItem() ? slot.Item.Amount : 0;
                    count.SetText(amount.ToString());
                    SetVisible(true);
                }
            });
        }

        private void SetVisible(bool visible)
        {
            image.gameObject.SetActive(visible);
            count.gameObject.SetActive(visible);
        }
    }
}
