using System.Collections.Generic;
using Balancy;
using Balancy.Example;
using Balancy.Models.BalancyShop;
using Balancy.Models.GameShop;
using UnityEngine;

namespace BalancyShop
{
    public class BottomController : MonoBehaviour
    {
        [SerializeField] private GameObject buttonPrefab;
        [SerializeField] private RectTransform content;

        private readonly List<ControlButton> _allButtons = new List<ControlButton>();
        private System.Action<WindowType> _onSectionChanged;

        public void Init(System.Action<WindowType> onSectionChanged, WindowType selectedWindowType)
        {
            _onSectionChanged = onSectionChanged;
            Refresh(selectedWindowType);
        }
        
        private void Refresh(WindowType selectedWindowType)
        {
            content.RemoveChildren();
            _allButtons.Clear();

            var sections = DataEditor.BalancyShop.GameSections;
            foreach (var section in sections)
            {
                var newItem = Instantiate(buttonPrefab, content);
                var controlButton = newItem.GetComponent<ControlButton>();
                controlButton.Init(section, OnSectionSelected);
                if (section.Type == selectedWindowType)
                    controlButton.SetSelected(section);
                _allButtons.Add(controlButton);
            }
        }

        private void OnSectionSelected(GameSection section)
        {
            foreach (var button in _allButtons)
                button.SetSelected(section);

            _onSectionChanged?.Invoke(section.Type);
        }
    }
}
