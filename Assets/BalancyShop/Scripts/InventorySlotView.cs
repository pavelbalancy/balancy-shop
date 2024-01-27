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

        public void UpdateData(ItemInstance itemInstance)
        {
            SetVisible(false);
            (itemInstance?.Item as MyItem)?.Icon.LoadSprite(sprite =>
            {
                if (sprite != null)
                {
                    image.sprite = sprite;
                    count.SetText(itemInstance.Amount.ToString());
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
