using System;
using Balancy;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BalancyShop
{
    public class InventoryWindow : BaseWindow
    {
        [SerializeField] private TMP_InputField playerLevel;
        [SerializeField] private Button resetButton;
        [SerializeField] private InventoryView inventoryResources;
        [SerializeField] private InventoryView inventoryItems;

        private void Start()
        {
            resetButton.onClick.AddListener(OnResetProfile);
            playerLevel.onValueChanged.AddListener(OnLevelChanged);
        }

        private void OnEnable()
        {
            UpdateLevelText();
            inventoryResources.Init(LiveOps.Profile.Inventories.Currencies);
            inventoryItems.Init(LiveOps.Profile.Inventories.Items);
        }

        private void OnLevelChanged(string newValue)
        {
            if (int.TryParse(newValue, out var intValue))
                LiveOps.Profile.GeneralInfo.Level = intValue;
            else
                Debug.Log($"Failed to parse {newValue} to int");
        }

        private void OnResetProfile()
        {
            LiveOps.Profile.Restart(() =>
            {
                Debug.Log("Profile was reset");
                OnEnable();
            });
        }

        private void UpdateLevelText()
        {
            playerLevel.text = LiveOps.Profile.GeneralInfo.Level.ToString();
        }
    }
}
