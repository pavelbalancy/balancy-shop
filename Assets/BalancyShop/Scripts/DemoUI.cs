using Balancy;
using Balancy.Models.GameShop;
using UnityEngine;

namespace BalancyShop
{
    public class DemoUI : MonoBehaviour
    {
        [SerializeField] private BottomController controller;
        [SerializeField] private WindowType defaultWindow;
        [SerializeField] private RectTransform windowsHolder;
        
        private BaseWindow _selectedWindow;
        private BaseWindow[] _allWindows;
        
        public void Init()
        {
            _allWindows = windowsHolder.GetComponentsInChildren<BaseWindow>();
            OnWindowSelected(defaultWindow);
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
    }
}
