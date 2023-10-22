using UnityEngine;

namespace Balancy.Example
{
    public class DailyBonusView : MonoBehaviour
    {
        [SerializeField] private RectTransform content;
        [SerializeField] private GameObject slotPrefab;
        
        public void Init()
        {
            var allRewards = LiveOps.DailyBonus.GetAllRewards();
            
            content.RemoveChildren();

            for (int i = 0; i < allRewards.Length; i++)
            {
                var newItem = Instantiate(slotPrefab, content);
                var bonusView = newItem.GetComponent<DailyBonusSlot>();
                bonusView.Init(i, allRewards[i]);
            }
        }
    }
}
