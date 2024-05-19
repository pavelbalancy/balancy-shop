using System;
using Balancy;
using Balancy.Data.SmartObjects;
using Balancy.Example;
using Balancy.Models.SmartObjects;
using UnityEngine;
using UnityEngine.UI;

namespace BalancyShop
{
    public class GameEventsWindow : BaseWindow
    {
        [SerializeField] private RectTransform contentActive;
        [SerializeField] private RectTransform contentScheduled;
        [SerializeField] private GameObject eventPrefab;
        [SerializeField] private ForceTransformUpdater layout;

        private void Start()
        {
            BalancyShopSmartObjectsEvents.onEventActivated += OnEventChanged;
            BalancyShopSmartObjectsEvents.onEventDeactivated += OnEventChanged;
        }
        
        private void OnDestroy()
        {
            BalancyShopSmartObjectsEvents.onEventActivated -= OnEventChanged;
            BalancyShopSmartObjectsEvents.onEventDeactivated -= OnEventChanged;
        }

        private void OnEventChanged(EventInfo obj)
        {
            Refresh();
        }

        public override void Refresh()
        {
            contentActive.RemoveChildren();
            contentScheduled.RemoveChildren();

            var allMyEvents = DataEditor.MyGameEvents;
            foreach (var myEvent in allMyEvents)
            {
                bool isActive = IsEventActive(myEvent);
                var trm = isActive ? contentActive : contentScheduled;
                var newEvent = Instantiate(eventPrefab, trm);
                newEvent.GetComponent<GameEventInfo>()?.Init(myEvent, isActive);
            }
            
            layout.ForceUpdate();
        }

        private bool IsEventActive(GameEvent gameEvent)
        {
            var allActiveEvents = LiveOps.GameEvents.GetActiveEvents();
            foreach (var ev in allActiveEvents)
            {
                if (ev.GameEvent == gameEvent)
                    return true;
            }

            return false;
        }
    }
}
