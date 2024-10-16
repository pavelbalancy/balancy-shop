using Balancy;
using Balancy.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BalancyShop
{
    public class GameEventInfo : MonoBehaviour
    {
        private const int TIMER_REFRESH = 1;
        
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text name;
        [SerializeField] private TMP_Text timer;

        private MyGameEvent _gameEvent;
        private bool _isActive;

        public void Init(MyGameEvent gameEvent, bool isActive)
        {
            _gameEvent = gameEvent;
            _isActive = isActive;
            name.text = gameEvent.Name.Value;
            gameEvent.Icon.LoadSprite(sprite =>
            {
                if (!icon.IsDestroyed())
                    icon.sprite = sprite;
            });
            BalancyTimer.SubscribeForTimer(TIMER_REFRESH, Refresh);
            Refresh();
        }

        private void OnDestroy()
        {
            BalancyTimer.UnsubscribeFromTimer(TIMER_REFRESH, Refresh);
        }

        private void Refresh()
        {
            if (_isActive)
            {
                var timeLeft = _gameEvent.GetSecondsLeftBeforeDeactivation();
                if (timeLeft == int.MaxValue)
                    timer.text = "Infinite";
                else
                    timer.text = GameUtils.FormatTime(timeLeft);
            }
            else
                timer.text = GameUtils.FormatTime(_gameEvent.GetSecondsBeforeActivation());
        }
    }
}