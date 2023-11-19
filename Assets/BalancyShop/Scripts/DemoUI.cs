using Balancy.Data.SmartObjects;
using Balancy.Models.GameShop;
using UnityEngine;

namespace BalancyShop
{
    public class DemoUI : MonoBehaviour
    {
        [SerializeField] private BottomController controller;
        [SerializeField] private WindowType defaultWindow;
        [SerializeField] private RectTransform windowsHolder;
        
        [SerializeField] private OfferPopup offerPopup;
        
        private BaseWindow _selectedWindow;
        private BaseWindow[] _allWindows;

        private static DemoUI _instance;
        
        public void Init()
        {
            _allWindows = windowsHolder.GetComponentsInChildren<BaseWindow>();
            OnWindowSelected(defaultWindow);
            offerPopup.Hide();
            _instance = this;
        }

        public void Refresh()
        {
            _selectedWindow?.Refresh();
            controller.Init(OnWindowSelected, _selectedWindow.winType);
        }

        private void OnWindowSelected(WindowType windowType)
        {
            foreach (var window in _allWindows)
            {
                if (window.winType == windowType)
                    _selectedWindow = window;
                window.SetActive(window.winType == windowType);
            }
        }

        public static void ShowRandomOffer()
        {
            var allOffers = Balancy.LiveOps.GameOffers.GetActiveOffers();
            if (allOffers.Length <= 0)
                return;
            
            int rndIndex = Random.Range(0, allOffers.Length);
            var rndOffer = allOffers[rndIndex];

            _instance?.ShowOffer(rndOffer);
        }
        
        private void ShowOffer(OfferInfo offerInfo)
        {
            offerPopup.Show(offerInfo);
        }
    }
}
