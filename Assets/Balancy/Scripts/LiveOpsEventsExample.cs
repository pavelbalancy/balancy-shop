using System;
using Balancy.Interfaces;
using Balancy.Models.SmartObjects;
using UnityEngine;

namespace Balancy
{
    //TOTO make your own version of this file, because the original file will be overwritten after Balancy update
    public class LiveOpsEventsExample : ILiveOpsEvents
    {
        public static event Action<Reward, int> OnDailyRewardAvailableEvent; 
        
        public void OnDailyRewardAvailable(Reward reward, int dayNumber)
        {
            Debug.Log("=> OnDailyRewardAvailable: " + dayNumber);
            OnDailyRewardAvailableEvent?.Invoke(reward, dayNumber);
        }
    }
}
