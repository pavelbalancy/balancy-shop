using Balancy;
using Balancy.Example;
using UnityEngine;
using UnityEngine.UI;

namespace BalancyShop
{
    public class BattlePassWindow : BaseWindow
    {
        [SerializeField] private RectTransform content;
        [SerializeField] private GameObject prefab;
        [SerializeField] private ForceTransformUpdater layout;
        [SerializeField] private Image progressSprite;
        
        public override void Refresh()
        {
            content.RemoveChildren();

            var activeBattlePass = BattlePassExample.GetInstance()?.GetActiveBattlePass();
            if (activeBattlePass == null)
                return;
            
            foreach (var point in activeBattlePass.Points)
            {
                var newPoint = Instantiate(prefab, content);
                var view = newPoint.GetComponent<BattlePointView>();
                view.Init(point);
            }

            progressSprite.fillAmount =
                (float)Mathf.Clamp(LiveOps.Profile.GeneralInfo.Level, 0, activeBattlePass.Points.Length) /
                activeBattlePass.Points.Length; 
            
            layout.ForceUpdate();
        }
    }
}
