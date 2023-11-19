using Balancy;
using Balancy.Data.SmartObjects;
using Balancy.Models.BalancyShop;
using UnityEngine;
using UnityEngine.UI;

namespace BalancyShop
{
    public class OfferPopup : MonoBehaviour
    {
        [SerializeField]
        private SlotView slotView;
        
        [SerializeField]
        private Button closeButton;


        private OfferInfo _offerInfo;
        public void Show(OfferInfo offerInfo)
        {
            slotView.Init(offerInfo, (offerInfo.GameOffer as MyOffer)?.UIPopupData);
            gameObject.SetActive(true);

            _offerInfo = offerInfo;

            BalancyShopSmartObjectsEvents.onOfferDeactivated += OnOfferDeactivated;
        }

        private void OnOfferDeactivated(OfferInfo offerInfo)
        {
            if (offerInfo == _offerInfo)
                Hide();
        }

        private void Start()
        {
            closeButton.onClick.AddListener(Hide);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            BalancyShopSmartObjectsEvents.onOfferDeactivated -= OnOfferDeactivated;
        }
    }
}