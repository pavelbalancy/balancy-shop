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

        private void Start()
        {
            resetButton.onClick.AddListener(OnResetProfile);
            playerLevel.onValueChanged.AddListener(OnLevelChanged);
        }

        private void OnEnable()
        {
            UpdateLevelText();
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
                UpdateLevelText();
            });
        }

        private void UpdateLevelText()
        {
            playerLevel.text = LiveOps.Profile.GeneralInfo.Level.ToString();
        }
    }
}
