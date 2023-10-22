#if !BALANCY_SERVER
using Balancy.Data;
using Balancy.Data.SmartObjects;
using Balancy.Models.SmartObjects;
using Balancy.SmartObjects;
using UnityEngine;

namespace Balancy
{
    //TOTO make your own version of this file, because the original file will be overwritten after Balancy update
    public class SmartObjectsEventsExample : ISmartObjectsEvents
    {
        public void OnSystemProfileConflictAppeared()
        {
            Debug.Log("=> OnSystemProfileConflictAppeared");
            // System Profile is created and handled automatically. It contains Scripts progress, active Events and Offers, information about A/B testing, Payments, Segmentations, etc...
            // Balancy.LiveOps.Profile.SolveConflict(ConflictsManager.VersionType.Local);
            Balancy.LiveOps.Profile.SolveConflict(ConflictsManager.VersionType.Cloud);
        }

        public void OnNewOfferActivated(OfferInfo offerInfo)
        {
            Debug.Log("=> OnNewOfferActivated: " + offerInfo?.GameOffer?.Name + " ; Price = " + offerInfo?.PriceUSD + " ; Discount = " + offerInfo?.Discount);
        }

        public void OnNewOfferGroupActivated(OfferGroupInfo offerInfo)
        {
            Debug.Log("=> OnNewOfferGroupActivated: " + offerInfo?.GameOfferGroup?.Name);
        }

        public void OnOfferDeactivated(OfferInfo offerInfo, bool wasPurchased)
        {
            Debug.Log("=> OnOfferDeactivated: " + offerInfo?.GameOffer?.Name + " ; wasPurchased = " + wasPurchased);
        }

        public void OnOfferGroupDeactivated(OfferGroupInfo offerInfo, bool wasPurchased)
        {
            Debug.Log("=> OnOfferGroupDeactivated: " + offerInfo?.GameOfferGroup?.Name + " ; wasPurchased = " + wasPurchased);
        }

        public void OnNewEventActivated(EventInfo eventInfo)
        {
            Debug.Log("=> OnNewEventActivated: " + eventInfo?.GameEvent?.Name);
        }

        public void OnEventDeactivated(EventInfo eventInfo)
        {
            Debug.Log("=> OnEventDeactivated: " + eventInfo?.GameEvent?.Name);
        }

        public void OnOfferPurchased(OfferInfo offerInfo)
        {
            Debug.Log("=> OnOfferPurchased: " + offerInfo?.GameOffer?.Name);
        }

        public void OnOfferGroupPurchased(OfferGroupInfo offerInfo, StoreItem storeItem)
        {
            Debug.Log("=> OnOfferGroupPurchased: " + offerInfo?.GameOfferGroup?.Name + " : storeItem = " + storeItem?.Name);
        }

        public void OnOfferFailedToPurchase(OfferInfo offerInfo, string error)
        {
            Debug.Log("=> OnOfferFailedToPurchase: " + offerInfo?.GameOffer?.Name + " ; Error = " + error);
        }

        public void OnStoreItemFailedToPurchase(StoreItem storeItem, string error)
        {
            Debug.Log("=> OnOfferFailedToPurchase: " + storeItem?.Name + " ; Error = " + error);
        }

        public void OnSegmentUpdated(SegmentInfo segmentInfo)
        {
            Debug.Log("=> OnSegmentUpdated: " + segmentInfo?.Segment?.Name + " ; IsIn = " + segmentInfo?.IsIn);
        }

        public void OnSmartObjectsInitialized()
        {
            Debug.Log("=> OnSmartObjectsInitialized:  You can now make purchase, request all GameEvents, GameOffers, A/B Tests, etc...");
        }

        public void OnAbTestStarted(LiveOps.ABTests.TestData abTestInfo)
        {
            Debug.Log("=> OnAbTestStarted: " + abTestInfo?.AbTest?.Name + " ; Variant = " + abTestInfo?.Variant?.Name);
        }

        public void OnAbTestEnded(LiveOps.ABTests.TestData abTestInfo)
        {
            Debug.Log("=> OnAbTestEnded: " + abTestInfo?.AbTest?.Name);
        }
    }
}
#endif
